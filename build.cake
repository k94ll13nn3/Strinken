//////////////////////////////////////////////////////////////////////
// DEPENDENCIES
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=GitVersion.CommandLine"

#addin "Cake.Git"
using System.Reflection

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = Argument("solution", "Strinken/");
var framework = Argument("framework", "netstandard1.1");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/") + Directory(solution) + Directory("bin") + Directory(configuration) + Directory(framework);
var publishDir = Directory("./artifacts");
var versionSuffix = "";
var nugetVersion = "";
var isOnMaster = IsRunningOnWindows() ? GitBranchCurrent(Directory(".")).FriendlyName == "master" : false;
var isOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
	CleanDirectory(publishDir);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>DotNetCoreRestore());

Task("Update-Assembly-Info")
    .IsDependentOn("Restore")
    .Does(() =>
{
  // does not currently run on mono 4.3.2, see https://github.com/GitTools/GitVersion/pull/890
  if(IsRunningOnWindows())
  {
      var version = GitVersion(new GitVersionSettings {
	    UpdateAssemblyInfo = true
      });

      nugetVersion = version.NuGetVersion;
    if(version.CommitsSinceVersionSource != "0")
    {
        versionSuffix = "ci" + version.CommitsSinceVersionSource.PadLeft(4, '0');
    }    
    if (isOnAppVeyor)
    {
        AppVeyor.UpdateBuildVersion(nugetVersion);
    }
  }
});

Task("Build")
    .IsDependentOn("Update-Assembly-Info")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = configuration
    };
            
    DotNetCoreBuild("./src/" + solution, settings);
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration
    };

    DotNetCoreTest("./test/Strinken.Tests/", settings);

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        Information("Uploading test results");
        var baseUri = EnvironmentVariable("APPVEYOR_URL").TrimEnd('/');
        var url = string.Format("{0}/api/testresults/nunit3/{1}", baseUri, AppVeyor.Environment.JobId);
        
        using (var r = new System.Net.WebClient())
        {
            r.UploadFile(url, "TestResult.xml");
        }     
    }
 });

Task("Display-Build-Info")
.IsDependentOn("Run-Unit-Tests")
.Does(() => {
    Information("Public members:");
    Assembly a = Assembly.LoadFrom("./src/" + solution + "/bin/" + configuration + "/" + framework + "/Strinken.dll");
    Type[] types = a.GetTypes();
    foreach (Type type in types)
    {
        if (!type.IsPublic) continue;
        MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
        foreach (MemberInfo member in members) Console.WriteLine("\t" + type.Name + "." + member.Name);
    }
    var assemblyInfo = ParseAssemblyInfo("./src/Strinken/Properties/AssemblyInfo.cs");
    Information("Version:");
    Information("\tAssembly Version: {0}", assemblyInfo.AssemblyVersion);
    Information("\tFile version: {0}", assemblyInfo.AssemblyFileVersion);
    Information("\tInformational version: {0}", assemblyInfo.AssemblyInformationalVersion);
});

Task("Nuget-Pack")
    .IsDependentOn("Display-Build-Info")
    .Does(() =>
{
if  (isOnAppVeyor && isOnMaster)
{
    EnsureDirectoryExists(publishDir);
    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = "./artifacts/"
    };

    if(versionSuffix != "")
    {
        settings.VersionSuffix = versionSuffix;
    }

    DotNetCorePack("./src/" + solution, settings);
}
});

Task("Upload-Artifact")
    .IsDependentOn("Nuget-Pack")
    .Does(() =>
{
    if (isOnAppVeyor && isOnMaster)
    {
        AppVeyor.UploadArtifact("./artifacts/Strinken." + nugetVersion +".nupkg");
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Upload-Artifact");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
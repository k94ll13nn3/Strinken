//////////////////////////////////////////////////////////////////////
// DEPENDENCIES
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=GitVersion.CommandLine"

using System.Reflection

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = Argument("solution", "Strinken/");
var framework = Argument("framework", "netstandard1.0");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/") + Directory(solution) + Directory("bin");
var publishDir = Directory("./artifacts");
var versionSuffix = "";
var nugetVersion = "";
var isOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isOnMaster = isOnAppVeyor ? EnvironmentVariable("APPVEYOR_REPO_BRANCH") == "master" : false;

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
    .WithCriteria(() => IsRunningOnWindows() && isOnAppVeyor)
    .IsDependentOn("Restore")
    .Does(() =>
{
    Information("Current branch:      " + (isOnAppVeyor ? EnvironmentVariable("APPVEYOR_REPO_BRANCH") : "----"));
    Information("Master branch:       " + isOnMaster.ToString());
    Information("Running on AppVeyor: " + isOnAppVeyor.ToString());
    Information("Running on Windows:  " + IsRunningOnWindows().ToString());
    // does not currently run on mono 4.3.2, see https://github.com/GitTools/GitVersion/pull/890
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
});

Task("Upload-Tests")
    .WithCriteria(() => isOnAppVeyor)
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    var baseUri = EnvironmentVariable("APPVEYOR_URL").TrimEnd('/');
    var url = string.Format("{0}/api/testresults/nunit3/{1}", baseUri, AppVeyor.Environment.JobId);

    using (var r = new System.Net.WebClient())
    {
        r.UploadFile(url, "TestResult.xml");
    }     
});

Task("Display-Build-Info")
.IsDependentOn("Upload-Tests")
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
    .WithCriteria(() => isOnMaster)
    .IsDependentOn("Display-Build-Info")
    .Does(() =>
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
});

Task("Upload-Artifact")
    .WithCriteria(() => isOnAppVeyor && isOnMaster)
    .IsDependentOn("Nuget-Pack")
    .Does(() =>
{
    AppVeyor.UploadArtifact("./artifacts/Strinken." + nugetVersion +".nupkg");
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
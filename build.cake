//////////////////////////////////////////////////////////////////////
// DEPENDENCIES
//////////////////////////////////////////////////////////////////////

#tool nuget:?package=GitVersion.CommandLine&version=3.6.2

using System.Reflection
using System.Diagnostics

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
var isPullRequest = isOnAppVeyor ? !string.IsNullOrEmpty(EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER")) : false;

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
    Information("Pull Request:        " + isPullRequest.ToString());
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
        Information("Build version:       " + nugetVersion + " (" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER") + ")");
        AppVeyor.UpdateBuildVersion(nugetVersion + " (" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER") + ")");
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
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("-xml \"TestResult.xml\"")
    };

    DotNetCoreTest("./test/Strinken.Tests/", settings);
    MoveFile("TestResult.xml", "TestResult.first.xml");
    DotNetCoreTest("./test/Strinken.Public.Tests/", settings);
});

Task("Upload-Tests")
    .WithCriteria(() => isOnAppVeyor)
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    AppVeyor.UploadTestResults("TestResult.first.xml", AppVeyorTestResultsType.XUnit);   
    AppVeyor.UploadTestResults("TestResult.xml", AppVeyorTestResultsType.XUnit);   
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
        MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Static);
        foreach (MemberInfo member in members) Console.WriteLine("\t" + type.Name + "." + member.Name);
    }
    Information("Version:");
    Information("\tAssembly Version: {0}", a.GetName().Version.ToString());
    Information("\tFile Version: {0}", FileVersionInfo.GetVersionInfo(a.Location).FileVersion);
    Information("\tInformational Version: {0}", FileVersionInfo.GetVersionInfo(a.Location).ProductVersion);
});

Task("Nuget-Pack")
    .WithCriteria(() => isOnMaster && !isPullRequest)
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
    .WithCriteria(() => isOnAppVeyor && isOnMaster && !isPullRequest)
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
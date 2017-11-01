//////////////////////////////////////////////////////////////////////
// DEPENDENCIES
//////////////////////////////////////////////////////////////////////

#tool GitVersion.CommandLine&version=3.6.5
#tool OpenCover&version=4.6.519
#tool GitReleaseNotes&version=0.7.0
#tool coveralls.io&version=1.4.2

#addin Cake.Coveralls&version=0.7.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var solutionDir = Directory("./src/") + Directory("Strinken/");
var buildDir = solutionDir + Directory("bin/");
var publishDir = Directory("./artifacts");
var coverageDir = Directory("./coverage");

// Define script variables
var releaseNotesPath = new FilePath("releaseNotes.md");
var coverageResultPath = new FilePath("./coverage.xml");
var framework = "netstandard1.0";
var versionSuffix = "";
var nugetVersion = "";
var currentBranch = EnvironmentVariable("APPVEYOR_REPO_BRANCH");
var isOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isOnMaster =  currentBranch == "master";
var isPullRequest = isOnAppVeyor ? AppVeyor.Environment.PullRequest.IsPullRequest : false;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
	CleanDirectory(publishDir);
	CleanDirectory(coverageDir);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => DotNetCoreRestore());

Task("Set-Environment")
    .IsDependentOn("Restore")
    .Does(() =>
{
    Information("Current branch:      " + currentBranch);
    Information("Master branch:       " + isOnMaster.ToString());
    Information("Pull Request:        " + isPullRequest.ToString());
    Information("Running on AppVeyor: " + isOnAppVeyor.ToString());

    var version = GitVersion();
    nugetVersion = version.NuGetVersion;
    if(version.CommitsSinceVersionSource.HasValue && version.CommitsSinceVersionSource.Value != 0)
    {
        versionSuffix = "ci" + version.CommitsSinceVersionSource?.ToString()?.PadLeft(4, '0');
    }    

    if (isOnAppVeyor)
    {
        Information("Build version:       " + nugetVersion + " (" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER") + ")");
        AppVeyor.UpdateBuildVersion(nugetVersion + " (" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER") + ")");
    }
});

Task("Build")
    .IsDependentOn("Set-Environment")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        VersionSuffix = versionSuffix
    };

    DotNetCoreBuild(solutionDir, settings);
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    EnsureDirectoryExists(coverageDir);
    var settings = new DotNetCoreTestSettings
    {
        Configuration = "Coverage"
    };

    var coverageSettings = new OpenCoverSettings().WithFilter("+[Strinken*]*").WithFilter("-[Strinken.Tests]*").WithFilter("-[Strinken.Public.Tests]*");
    coverageSettings.ReturnTargetCodeOffset = 1000; // Offset in order to have Cake fail if a test is a failure
    coverageSettings.Register = "user";
    coverageSettings.MergeOutput = true;
    coverageSettings.OldStyle = true;
    coverageSettings.SkipAutoProps = true;

    OpenCover(tool => {
            tool.DotNetCoreTest("./test/Strinken.Tests/Strinken.Tests.csproj", settings);
        },
        coverageDir + coverageResultPath,
        coverageSettings
    );

    OpenCover(tool => {
            tool.DotNetCoreTest("./test/Strinken.Public.Tests/Strinken.Public.Tests.csproj", settings);
        },
        coverageDir + coverageResultPath,
        coverageSettings
    );
});

Task("Nuget-Pack")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    EnsureDirectoryExists(publishDir);
    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = publishDir,
        VersionSuffix = versionSuffix
    };

    DotNetCorePack(solutionDir, settings);
});

Task("Generate-Release-Notes")
    .WithCriteria(false)
    .IsDependentOn("Nuget-Pack")
    .Does(() =>
{
    GitReleaseNotes(publishDir + releaseNotesPath, new GitReleaseNotesSettings {
        WorkingDirectory         = ".",
        AllTags                  = true
    });
});

Task("Upload-Coverage")
    .WithCriteria(() => isOnAppVeyor && isOnMaster && !isPullRequest)
    .IsDependentOn("Generate-Release-Notes")
    .Does(() =>
{
    CoverallsIo(coverageDir + coverageResultPath, new CoverallsIoSettings
    {
        RepoToken = EnvironmentVariable("coveralls_token")
    });
});

Task("Upload-Artifact")
    .WithCriteria(() => isOnAppVeyor && isOnMaster && !isPullRequest)
    .IsDependentOn("Upload-Coverage")
    .Does(() =>
{
    //AppVeyor.UploadArtifact(publishDir + releaseNotesPath);
    AppVeyor.UploadArtifact(publishDir + new FilePath("Strinken." + nugetVersion +".nupkg"));
});
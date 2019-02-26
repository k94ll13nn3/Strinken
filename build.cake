//////////////////////////////////////////////////////////////////////
// DEPENDENCIES
//////////////////////////////////////////////////////////////////////

#tool GitVersion.CommandLine&version=4.0.0-beta0014
#tool OpenCover&version=4.6.519
#tool coveralls.io&version=1.4.2
#tool Wyam&version=2.2.2
#tool KuduSync.NET&version=1.5.2

#addin Cake.Coveralls&version=0.9.0
#addin Octokit&version=0.32.0
#addin Cake.FileHelpers&version=3.1.0
#addin Cake.Wyam&version=2.2.2
#addin Cake.Git&version=0.19.0
#addin Cake.Kudu&version=0.8.0

//////////////////////////////////////////////////////////////////////
// USINGS
//////////////////////////////////////////////////////////////////////

using Octokit;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var configuration = Argument("configuration", "Release");
var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var solutionDir = Directory("./src/") + Directory("Strinken/");
var buildDir = solutionDir + Directory("bin/");
var publishDir = Directory("./artifacts");
var coverageDir = Directory("./coverage");
var outputPath = MakeAbsolute(Directory("./docs/output"));
var rootPublishFolder = MakeAbsolute(Directory("./docs/publish"));

// Define script variables
var releaseNotesPath = new FilePath("CHANGELOG.md");
var coverageResultPath = new FilePath("coverage.xml");
var versionSuffix = "";
var nugetVersion = "";
var currentBranch = EnvironmentVariable("APPVEYOR_REPO_BRANCH");
var isOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isOnMaster =  currentBranch == "master";
var isPullRequest = isOnAppVeyor ? AppVeyor.Environment.PullRequest.IsPullRequest : false;

var accessToken = EnvironmentVariable("GITHUB_TOKEN");
var deployRemote = @"https://github.com/k94ll13nn3/Strinken.git";
var deployBranch = "gh-pages";
var sourceCommit = GitLogTip("./");

var generateDocumentation = sourceCommit.Message.Contains("[build-doc]");
var isOnWindows = Context.Environment.Platform.Family == PlatformFamily.Windows;

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

Task("Set-Environment")
    .IsDependentOn("Clean")
    .WithCriteria(isOnWindows, "Not running on Windows.")
    .Does(() =>
{
    Information("Current branch:      " + currentBranch);
    Information("Master branch:       " + isOnMaster.ToString());
    Information("Pull Request:        " + isPullRequest.ToString());
    Information("Running on AppVeyor: " + isOnAppVeyor.ToString());

    var version = GitVersion(new GitVersionSettings 
    {
        UpdateAssemblyInfo = true, 
        WorkingDirectory = solutionDir 
    });
    
    nugetVersion = version.NuGetVersion;
    versionSuffix = version.NuGetVersion.Split("-").Skip(1).FirstOrDefault() ?? "";
    Information("VersionSuffix        " + versionSuffix);

    Information("AssembyVersion       " + version.AssemblySemVer);
    Information("FileVersion          " + version.AssemblySemFileVer);
    Information("InformationalVersion " + version.InformationalVersion);
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
    DotNetCoreBuild(solutionDir, new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        VersionSuffix = versionSuffix
    });
});

Task("Run-Unit-Tests-Windows")
    .IsDependentOn("Build")
    .WithCriteria(isOnWindows, "Not running on Windows.")
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

Task("Run-Unit-Tests-Linux")
    .IsDependentOn("Build")
    .WithCriteria(!isOnWindows, "Running on Windows.")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration
    };

    DotNetCoreTest("./test/Strinken.Tests/Strinken.Tests.csproj", settings);
    DotNetCoreTest("./test/Strinken.Public.Tests/Strinken.Public.Tests.csproj", settings);
});

Task("Nuget-Pack")
    .IsDependentOn("Run-Unit-Tests-Windows")
    .IsDependentOn("Run-Unit-Tests-Linux")
    .WithCriteria(isOnWindows, "Not running on Windows.")
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
    .WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("GITHUB_TOKEN")), "Environment variable \"GITHUB_TOKEN\" not set.")
    .WithCriteria(isOnWindows, "Not running on Windows.")
    .IsDependentOn("Nuget-Pack")
    .Does(async () => 
{
    var owner = "k94ll13nn3";
    var project = "Strinken";
    var client = new GitHubClient(new ProductHeaderValue($"{owner}.{project}"))
    {
        Credentials = new Credentials(EnvironmentVariable("GITHUB_TOKEN")),
    };

    var releases = await client.Repository.Release.GetAll(owner, project);
    var allIssues = await client.Issue.GetAllForRepository(owner, project, new RepositoryIssueRequest { State = ItemStateFilter.Closed });
    var excludedLabels = new[] { "duplicate", "invalid", "wontfix", "internal" };
    var issues = allIssues.Where(x => x.PullRequest == null && !x.Labels.Select(l => l.Name).Intersect(excludedLabels).Any());
    var pullRequestsLabels = allIssues
        .Where(x => x.PullRequest != null)
        .ToDictionary(x => x.Number, x => x.Labels.Select(l => l.Name));
    var pullRequests = (await client.PullRequest.GetAllForRepository(owner, project, new PullRequestRequest { State = ItemStateFilter.Closed }))
        .Where(x => x.Merged && !pullRequestsLabels[x.Number].Intersect(excludedLabels).Any());
    var links = releases.Zip(releases.Skip(1), (a, b) => $"[{a.Name}]: https://github.com/{owner}/{project}/compare/{b.TagName}...{a.TagName}").ToList();

    var builder = new StringBuilder();
    builder.AppendLine("# Changelog").AppendLine();
    builder.AppendLine("All notable changes to this project will be documented in this file.").AppendLine();
    builder.AppendLine("The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)");
    builder.AppendLine("and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).").AppendLine();

    releases = releases.Append(new Release(null, null, null, null, 0, null, null, null, "Unreleased", null, false, false, DateTime.Now, DateTime.Now, null, null, null, null))
        .OrderByDescending(x => x.PublishedAt)
        .ToList();
    for (int i = 0; i < releases.Count - 1; i++)
    {
        var issuesForRelease = issues.Where(x => x.ClosedAt <= releases[i].PublishedAt && x.ClosedAt > releases[i + 1].PublishedAt).Select(FormatIssue);
        var pullRequestsForRelease = pullRequests.Where(x => x.ClosedAt <= releases[i].PublishedAt && x.ClosedAt > releases[i + 1].PublishedAt).Select(FormatPullRequest);

        if (!string.IsNullOrWhiteSpace(releases[i].Body) || issuesForRelease.Any() || pullRequestsForRelease.Any())
        {
            builder.AppendLine(FormatRelease(releases[i])).AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(releases[i].Body))
        {
            builder.AppendLine(releases[i].Body).AppendLine();
        }

        if (issuesForRelease.Any())
        {
            builder.AppendLine("### Issues").AppendLine();
            builder.Append(string.Join($"{Environment.NewLine}", issuesForRelease)).AppendLine().AppendLine();
        }
        if (pullRequestsForRelease.Any())
        {
            builder.AppendLine("### Pull Requests").AppendLine();
            builder.Append(string.Join($"{Environment.NewLine}", pullRequestsForRelease)).AppendLine().AppendLine();
        }
    }

    builder.AppendLine($"## {releases.Last().Name} - {releases.Last().PublishedAt.Value.ToString("yyyy'-'MM'-'dd")}").AppendLine();
    builder.AppendLine(releases.Last().Body).AppendLine();

    builder.Append(string.Join($"{Environment.NewLine}", links));

    FileWriteText(publishDir + releaseNotesPath, builder.ToString());

    string FormatRelease(Release release) => release.Name == "Unreleased" ? $"## {release.Name}" : $"## [{release.Name}] - {release.PublishedAt.Value.ToString("yyyy'-'MM'-'dd")}";
    string FormatIssue(Issue issue) => $"- [#{issue.Number}](https://github.com/{owner}/{project}/issues/{issue.Number}): {issue.Title}";
    string FormatPullRequest(PullRequest pullRequest) => $"- [#{pullRequest.Number}](https://github.com/{owner}/{project}/pull/{pullRequest.Number}): {pullRequest.Title} (by [{pullRequest.User.Login}](https://github.com/{pullRequest.User.Login}))";
});

Task("Upload-Coverage")
    .ContinueOnError()
    .WithCriteria(() => isOnAppVeyor && isOnMaster && !isPullRequest)
    .WithCriteria(isOnWindows, "Not running on Windows.")
    .IsDependentOn("Generate-Release-Notes")
    .Does(() =>
{
    CoverallsIo(coverageDir + coverageResultPath, new CoverallsIoSettings
    {
        RepoToken = EnvironmentVariable("COVERALLS_TOKEN")
    });
});

Task("Upload-Artifact")
    .WithCriteria(() => isOnAppVeyor && isOnMaster && !isPullRequest)
    .WithCriteria(isOnWindows, "Not running on Windows.")
    .IsDependentOn("Upload-Coverage")
    .Does(() =>
{
    AppVeyor.UploadArtifact(publishDir + new FilePath("Strinken." + nugetVersion +".nupkg"));
    if (FileExists(publishDir + releaseNotesPath))
    {
        AppVeyor.UploadArtifact(publishDir + releaseNotesPath);
    }
});

Task("Build-Documentation")
    .WithCriteria(generateDocumentation, "[build-doc] not found in commit message.")
    .WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("GITHUB_TOKEN")), "Environment variable \"GITHUB_TOKEN\" not set.")
    .WithCriteria(isOnWindows, "Not running on Windows.")
    .ContinueOnError()
    .Does(() =>
{
    EnsureDirectoryExists(outputPath);
    EnsureDirectoryExists(rootPublishFolder);

    Wyam(new WyamSettings
    {
        RootPath = "docs"
    });

    if (isOnAppVeyor && !isPullRequest)
    {
        Information("Publishing documentation...");
        var publishFolder = rootPublishFolder.Combine(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        
        Information("Getting publish branch...");
        GitClone(deployRemote, publishFolder, new GitCloneSettings{ BranchName = deployBranch });

        Information("Sync output files...");
        Kudu.Sync(outputPath, publishFolder, new KuduSyncSettings { 
            ArgumentCustomization = args=>args.Append("--ignore").AppendQuoted(".git;CNAME")
        });

        Information("Stage all changes...");
        GitAddAll(publishFolder);

        Information("Commit all changes...");
        GitCommit(
            publishFolder,
            sourceCommit.Committer.Name,
            sourceCommit.Committer.Email,
            string.Format("AppVeyor Publish: {0}\r\n{1}", sourceCommit.Sha, sourceCommit.Message)
            );

        Information("Pushing all changes...");
        GitPush(publishFolder, accessToken, "x-oauth-basic", deployBranch);
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Upload-Artifact")
    .IsDependentOn("Build-Documentation");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

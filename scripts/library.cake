//////////////////////////////////////////////////////////////////////
// DEPENDENCIES
//////////////////////////////////////////////////////////////////////

#tool GitVersion.CommandLine&version=4.0.0-beta0012
#tool OpenCover&version=4.6.519
#tool coveralls.io&version=1.4.2

#addin Cake.Coveralls&version=0.9.0
#addin Octokit&version=0.31.0
#addin Cake.FileHelpers&version=3.0.0

//////////////////////////////////////////////////////////////////////
// USINGS
//////////////////////////////////////////////////////////////////////

using Octokit;

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
var releaseNotesPath = new FilePath("CHANGELOG.md");
var coverageResultPath = new FilePath("coverage.xml");
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

Task("Set-Environment")
    .IsDependentOn("Clean")
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
    if(version.CommitsSinceVersionSource.HasValue && version.CommitsSinceVersionSource.Value != 0)
    {
        versionSuffix = "ci" + version.CommitsSinceVersionSource?.ToString()?.PadLeft(4, '0');
    }    

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
    .ContinueOnError()
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

    releases = releases.Append(new Release(null, null, null, null, 0, null, null, "Unreleased", null, false, false, DateTime.Now, DateTime.Now, null, null, null, null))
        .OrderByDescending(x => x.PublishedAt)
        .ToList();
    for (int i = 0; i < releases.Count - 1; i++)
    {
        builder.AppendLine(FormatRelease(releases[i])).AppendLine();
        if (!string.IsNullOrWhiteSpace(releases[i].Body))
        {
            builder.AppendLine(releases[i].Body).AppendLine();
        }

        var issuesForRelease = issues.Where(x => x.ClosedAt <= releases[i].PublishedAt && x.ClosedAt > releases[i + 1].PublishedAt).Select(FormatIssue);
        if (issuesForRelease.Any())
        {
            builder.AppendLine("### Issues").AppendLine();
            builder.Append(string.Join($"{Environment.NewLine}", issuesForRelease)).AppendLine().AppendLine();
        }
        var pullRequestsForRelease = pullRequests.Where(x => x.ClosedAt <= releases[i].PublishedAt && x.ClosedAt > releases[i + 1].PublishedAt).Select(FormatPullRequest);
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
    .IsDependentOn("Upload-Coverage")
    .Does(() =>
{
    AppVeyor.UploadArtifact(publishDir + new FilePath("Strinken." + nugetVersion +".nupkg"));
    if (FileExists(publishDir + releaseNotesPath))
    {
        AppVeyor.UploadArtifact(publishDir + releaseNotesPath);
    }
});

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace Strinken.Build
{
    public static class Program
    {
        private static readonly string ArtifactsPath = Path.GetFullPath("./artifacts");
        private static readonly string OutputPath = Path.GetFullPath("./docs/output");
        private static readonly string RootPublishFolder = Path.GetFullPath("./docs/publish");
        private static readonly bool IsRunningOnAppVeyor = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR"));
        private static readonly bool IsRunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static async Task Main(string[] args)
        {
            Target(
                "clean",
                () => DeleteDirectoryIfExists(ArtifactsPath));

            Target(
                "build",
                 DependsOn("clean"),
                () => RunAsync("dotnet", $"build --configuration Release /nologo /p:PackageOutputPath={ArtifactsPath}"));

            Target(
                "tests",
                 DependsOn("build"),
                 () => Run("dotnet", "test --configuration Release"));

            Target(
                "notes",
                async () => await GenerateReleaseNotes(ArtifactsPath));

            Target(
                "doc",
                () => GenerateDocumentation());

            Target(
                "coverage",
                () =>
                {
                    Run("dotnet", "tool update -g dotnet-reportgenerator-globaltool");
                    Run("dotnet", "tool update -g coverlet.console");
                    Run("dotnet", "build");
                    Run("coverlet", @".\tests\Strinken.Public.Tests\bin\Debug\netcoreapp2.2\Strinken.Public.Tests.dll --target ""dotnet"" --targetargs ""test .\tests\Strinken.Public.Tests\Strinken.Public.Tests.csproj --no-build"" --format cobertura --output coverage.public.xml");
                    Run("coverlet", @".\tests\Strinken.Tests\bin\Debug\netcoreapp2.2\Strinken.Tests.dll --target ""dotnet"" --targetargs ""test .\tests\Strinken.Tests\Strinken.Tests.csproj --no-build"" --format cobertura --output coverage.xml");
                    Run("reportgenerator", @"-reports:""coverage.xml;coverage.public.xml"" -targetdir:coverage -reporttypes:""HtmlInline_AzurePipelines""");
                });

            Target("default", DependsOn("tests", "notes", "doc"));

            await RunTargetsAndExitAsync(args);
        }

        private static void DeleteDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private static async Task GenerateReleaseNotes(string artifactsPath)
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_TOKEN")))
            {
                Console.WriteLine("Environment variable \"GITHUB_TOKEN\" not set.");
                return;
            }

            const string releaseNotesPath = "CHANGELOG.md";
            const string owner = "k94ll13nn3";
            const string project = "Strinken";
            var client = new GitHubClient(new ProductHeaderValue($"{owner}.{project}"))
            {
                Credentials = new Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN")),
            };

            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll(owner, project);
            IReadOnlyList<Issue> allIssues = await client.Issue.GetAllForRepository(owner, project, new RepositoryIssueRequest { State = ItemStateFilter.Closed });
            string[] excludedLabels = new[] { "duplicate", "invalid", "wontfix", "internal" };
            IEnumerable<Issue> issues = allIssues.Where(x => x.PullRequest == null && !x.Labels.Select(l => l.Name).Intersect(excludedLabels).Any());
            var pullRequestsLabels = allIssues
                .Where(x => x.PullRequest != null)
                .ToDictionary(x => x.Number, x => x.Labels.Select(l => l.Name));
            IEnumerable<PullRequest> pullRequests = (await client.PullRequest.GetAllForRepository(owner, project, new PullRequestRequest { State = ItemStateFilter.Closed }))
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
                IEnumerable<string> issuesForRelease = issues.Where(x => x.ClosedAt <= releases[i].PublishedAt && x.ClosedAt > releases[i + 1].PublishedAt).Select(FormatIssue);
                IEnumerable<string> pullRequestsForRelease = pullRequests.Where(x => x.ClosedAt <= releases[i].PublishedAt && x.ClosedAt > releases[i + 1].PublishedAt).Select(FormatPullRequest);

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
                    builder.AppendLine(string.Join($"{Environment.NewLine}", issuesForRelease)).AppendLine();
                }
                if (pullRequestsForRelease.Any())
                {
                    builder.AppendLine("### Pull Requests").AppendLine();
                    builder.AppendLine(string.Join($"{Environment.NewLine}", pullRequestsForRelease)).AppendLine();
                }
            }

            builder.AppendLine($"## {releases.Last().Name} - {releases.Last().PublishedAt.Value.ToString("yyyy'-'MM'-'dd")}").AppendLine();
            builder.AppendLine(releases.Last().Body).AppendLine();

            builder.Append(string.Join($"{Environment.NewLine}", links));

            File.WriteAllText(Path.Combine(artifactsPath, releaseNotesPath), builder.ToString());

            string FormatRelease(Release release) => release.Name == "Unreleased" ? $"## {release.Name}" : $"## [{release.Name}] - {release.PublishedAt.Value.ToString("yyyy'-'MM'-'dd")}";
            string FormatIssue(Issue issue) => $"- [#{issue.Number}](https://github.com/{owner}/{project}/issues/{issue.Number}): {issue.Title}";
            string FormatPullRequest(PullRequest pullRequest) => $"- [#{pullRequest.Number}](https://github.com/{owner}/{project}/pull/{pullRequest.Number}): {pullRequest.Title} (by [{pullRequest.User.Login}](https://github.com/{pullRequest.User.Login}))";
        }

        private static void GenerateDocumentation()
        {
            var lastCommitMessage = Read("git", "log -1 --pretty=format:%B");
            if (!lastCommitMessage.Contains("[build-doc]"))
            {
                Console.WriteLine("[build-doc] not found in commit message.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_TOKEN")))
            {
                Console.WriteLine("Environment variable \"GITHUB_TOKEN\" not set.");
                return;
            }

            if (!IsRunningOnAppVeyor)
            {
                Console.WriteLine("Not running on AppVeyor.");
                return;
            }

            if (!IsRunningOnWindows)
            {
                Console.WriteLine("Not running on Windows.");
                return;
            }

            DeleteDirectoryIfExists(OutputPath);
            DeleteDirectoryIfExists(RootPublishFolder);

            Run("dotnet", "tool install -g wyam.tool");
            Run("wyam", "build docs");

            var publishFolder = Path.Combine(RootPublishFolder, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            var deployRemote = @"https://github.com/k94ll13nn3/Strinken.git";
            var deployBranch = "gh-pages";
            var lastCommiterName = Read("git", "log -1 --pretty=format:%an");
            var lastCommiterEmail = Read("git", "log -1 --pretty=format:%ae");
            var lastCommitHash = Read("git", "log -1 --pretty=format:%h");

            Run("git", $"clone {deployRemote} -b {deployBranch} {publishFolder}");
            Run("git", "rm -rf .", publishFolder);
            Run("xcopy", @"..\..\output . /E", publishFolder);
            Run("git", "add .", publishFolder);
            Run("git", "config --global credential.helper store", publishFolder);
            Run("git", $"config --global user.email {lastCommiterEmail}", publishFolder);
            Run("git", $"config --global user.name {lastCommiterName}", publishFolder);
            Run("powershell", @"Add-Content ""$env:USERPROFILE\.git-credentials"" ""https://$($env:GITHUB_TOKEN):x-oauth-basic@github.com`n""", publishFolder);
            Run("git", $"commit -a -m \"AppVeyor Publish: {lastCommitHash}\r\n{lastCommitMessage}\"", publishFolder);
            Run("git", "push", publishFolder);
        }
    }
}

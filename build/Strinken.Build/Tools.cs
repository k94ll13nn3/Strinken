using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using static SimpleExec.Command;

namespace Strinken.Build
{
    public static class Tools
    {
        public static void DeleteDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public static async Task GenerateReleaseNotesAsync(string artifactsPath, string tokenName, string owner, string project)
        {
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER")))
            {
                Console.WriteLine("Not running on pull requests.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(tokenName)))
            {
                Console.WriteLine($"Environment variable \"{tokenName}\" not set.");
                return;
            }

            const string releaseNotesPath = "CHANGELOG.md";
            var client = new GitHubClient(new ProductHeaderValue($"{owner}.{project}"))
            {
                Credentials = new Credentials(Environment.GetEnvironmentVariable(tokenName)),
            };

            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll(owner, project);
            IReadOnlyList<Issue> allIssues = await client.Issue.GetAllForRepository(owner, project, new RepositoryIssueRequest { State = ItemStateFilter.Closed });
            string[] excludedLabels = new[] { "duplicate", "invalid", "wontfix", "internal", "dependencies" };
            IEnumerable<Issue> issues = allIssues.Where(x => x.PullRequest == null && !x.Labels.Select(l => l.Name).Intersect(excludedLabels).Any());
            var pullRequestsLabels = allIssues
                .Where(x => x.PullRequest is not null)
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
                    builder.AppendLine(FormatRelease(releases[i], true)).AppendLine();
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

            builder.AppendLine(FormatRelease(releases.Last(), false)).AppendLine();
            builder.AppendLine(releases.Last().Body).AppendLine();

            builder.Append(string.Join($"{Environment.NewLine}", links));

            File.WriteAllText(Path.Combine(artifactsPath, releaseNotesPath), builder.ToString());

            string FormatPullRequest(PullRequest pullRequest) => $"- [#{pullRequest.Number}](https://github.com/{owner}/{project}/pull/{pullRequest.Number}): {pullRequest.Title} (by [{pullRequest.User.Login}](https://github.com/{pullRequest.User.Login}))";
            string FormatIssue(Issue issue) => $"- [#{issue.Number}](https://github.com/{owner}/{project}/issues/{issue.Number}): {issue.Title}";
            static string FormatRelease(Release release, bool linked)
            {
                if (release.Name == "Unreleased")
                {
                    return "## Unreleased";
                }
                else
                {
                    string name = linked ? $"[{release.Name}]" : release.Name;
                    return $"## {name} - {release.PublishedAt?.ToString("yyyy'-'MM'-'dd")}";
                }
            }
        }

        public static void GenerateDocumentation(string documentationPath, string tokenName, string owner, string project)
        {
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER")))
            {
                Console.WriteLine("Not running on pull requests.");
                return;
            }

            string lastCommitMessage = Read("git", "log -1 --pretty=format:%B");
            if (!lastCommitMessage.Contains("[build-doc]"))
            {
                Console.WriteLine("[build-doc] not found in commit message.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(tokenName)))
            {
                Console.WriteLine($"Environment variable \"{tokenName}\" not set.");
                return;
            }

            bool isRunningOnAppVeyor = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR"));
            if (!isRunningOnAppVeyor)
            {
                Console.WriteLine("Not running on AppVeyor.");
                return;
            }

            bool isRunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!isRunningOnWindows)
            {
                Console.WriteLine("Not running on Windows.");
                return;
            }

            string outputPath = Path.GetFullPath($"./{documentationPath}output");
            string rootPublishFolder = Path.GetFullPath($"./{documentationPath}publish");

            DeleteDirectoryIfExists(outputPath);
            DeleteDirectoryIfExists(rootPublishFolder);

            Run("dotnet", "tool install -g wyam.tool");
            Run("wyam", $"build {documentationPath}");

            string publishFolder = Path.Combine(rootPublishFolder, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            string deployRemote = $"https://github.com/{owner}/{project}.git";
            const string deployBranch = "gh-pages";
            string lastCommiterName = Read("git", "log -1 --pretty=format:%an");
            string lastCommiterEmail = Read("git", "log -1 --pretty=format:%ae");
            string lastCommitHash = Read("git", "log -1 --pretty=format:%h");

            Run("git", $"clone {deployRemote} -b {deployBranch} {publishFolder}");
            Run("git", "rm -rf .", publishFolder);
            Run("xcopy", @"..\..\output . /E", publishFolder);
            Run("git", "add .", publishFolder);
            Run("git", "config --global credential.helper store", publishFolder);
            Run("git", $"config --global user.email {lastCommiterEmail}", publishFolder);
            Run("git", $"config --global user.name {lastCommiterName}", publishFolder);
            Run("powershell", $@"Add-Content ""$env:USERPROFILE\.git-credentials"" ""https://$($env:{tokenName}):x-oauth-basic@github.com`n""", publishFolder);
            Run("git", $"commit -a -m \"AppVeyor Publish: {lastCommitHash}\r\n{lastCommitMessage}\"", publishFolder);
            Run("git", "push", publishFolder);
        }
    }
}

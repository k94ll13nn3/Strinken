// Inspired from https://github.com/cake-contrib/Cake.Recipe/blob/develop/Cake.Recipe/Content/wyam.cake

//////////////////////////////////////////////////////////////////////
// DEPENDENCIES
//////////////////////////////////////////////////////////////////////

#tool Wyam&version=1.3.0
#tool KuduSync.NET&version=1.4.0

#addin Cake.Wyam&version=1.3.0
#addin Cake.Git&version=0.16.1
#addin Cake.Kudu&version=0.6.0

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var accessToken = EnvironmentVariable("access_token");
var deployRemote = @"https://github.com/k94ll13nn3/Strinken.git";
var deployBranch = "gh-pages";
var sourceCommit = GitLogTip("./");
var outputPath = MakeAbsolute(Directory("./docs/output"));
var rootPublishFolder = MakeAbsolute(Directory("./docs/publish"));
var shouldGenerateDocumentation = sourceCommit.Message.Contains("[build-doc]");
var forceDocumentation = sourceCommit.Message.Contains("[build-doc-force]"); // to rebuild when no file was changed

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean-Documentation")
    .Does(() =>
{
    EnsureDirectoryExists(outputPath);
    EnsureDirectoryExists(rootPublishFolder);
});

Task("Build-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(shouldGenerateDocumentation || forceDocumentation)
    .Does(() =>
{
    // Check to see if any documentation has changed
    var sourceCommit = GitLogTip("./");
    Information("Source Commit Sha: {0}", sourceCommit.Sha);
    var filesChanged = GitDiff("./", sourceCommit.Sha);
    Information("Number of changed files: {0}", filesChanged.Count);
    var docFileChanged = false;

    var wyamDocsFolderDirectoryName = "docs";
    foreach(var file in filesChanged)
    {
        var backslash = '\\';
        Verbose("Changed File OldPath: {0}, Path: {1}", file.OldPath, file.Path);
        if(file.OldPath.Contains(string.Format("{0}{1}", wyamDocsFolderDirectoryName, backslash)) || 
            file.Path.Contains(string.Format("{0}{1}", wyamDocsFolderDirectoryName, backslash)) ||
            file.Path.Contains("config.wyam"))
        {
           docFileChanged = true;
           break; 
        }
    }

    if(docFileChanged || forceDocumentation)
    {
        Information("Detected that documentation files have changed, so running Wyam...");
        
        Wyam(new WyamSettings
        {
            RootPath = "docs"
        });

        if (isOnAppVeyor && !isPullRequest)
        {
            Information("Publishing documentation...");
            PublishDocumentation();
        }
    }
    else
    {
        Information("No documentation has changed, so no need to generate documentation");
    }
});
    
public void PublishDocumentation()
{
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
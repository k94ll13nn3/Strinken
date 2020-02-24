using System.IO;
using System.Threading.Tasks;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace Strinken.Build
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            string artifactsPath = Path.GetFullPath("./artifacts");

            Target(
                "clean",
                () => Tools.DeleteDirectoryIfExists(artifactsPath));

            Target(
                "build",
                 DependsOn("clean"),
                () => RunAsync("dotnet", $"build --configuration Release /nologo /p:PackageOutputPath={artifactsPath}"));

            Target(
                "tests",
                 DependsOn("build"),
                 () => Run("dotnet", "test --configuration Release"));

            Target(
                "notes",
                async () => await Tools.GenerateReleaseNotesAsync(artifactsPath, "GITHUB_TOKEN", "k94ll13nn3", "Strinken"));

            Target(
                "doc",
                () => Tools.GenerateDocumentation("docs/", "GITHUB_TOKEN", "k94ll13nn3", "Strinken"));

            Target(
                "coverage",
                () =>
                {
                    Run("dotnet", "tool update -g dotnet-reportgenerator-globaltool");
                    Run("dotnet", "tool update -g coverlet.console");
                    Run("dotnet", "build");
                    Run("coverlet", @".\tests\Strinken.Public.Tests\bin\Debug\netcoreapp3.1\Strinken.Public.Tests.dll --target ""dotnet"" --targetargs ""test .\tests\Strinken.Public.Tests\Strinken.Public.Tests.csproj --no-build"" --format cobertura --output coverage.public.xml");
                    Run("coverlet", @".\tests\Strinken.Tests\bin\Debug\netcoreapp3.1\Strinken.Tests.dll --target ""dotnet"" --targetargs ""test .\tests\Strinken.Tests\Strinken.Tests.csproj --no-build"" --format cobertura --output coverage.xml");
                    Run("reportgenerator", @"-reports:""coverage.xml;coverage.public.xml"" -targetdir:coverage -reporttypes:""HtmlInline_AzurePipelines""");
                    Run("powershell", "ii coverage/index.htm");
                });

            Target(
                "format",
                () =>
                {
                    Run("dotnet", "tool update -g dotnet-format");
                    Run("dotnet", "format");
                });

            Target("default", DependsOn("tests", "notes", "doc"));

            await RunTargetsAndExitAsync(args);
        }
    }
}

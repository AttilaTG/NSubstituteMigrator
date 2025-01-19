using Spectre.Console;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NSubstituteMigrator.Migrator
{
    public class MigratorService
    {
        public List<string> GetTestProjects(string rootFolder)
        {
            var projectFiles = Directory.GetFiles(rootFolder, "*.csproj", SearchOption.AllDirectories);

            var testProjects = new List<string>();
            foreach (var project in projectFiles)
            {
                var xml = XDocument.Load(project);
                var isTestProject = xml.Descendants("IsTestProject")
                                       .FirstOrDefault()?.Value.Equals("true", StringComparison.OrdinalIgnoreCase);

                if (isTestProject == true)
                {
                    testProjects.Add(project);
                }
            }

            return testProjects;
        }

        public string SelectTestProject(List<string> testProjects)
        {
            var options = new List<string> { "All" };
            options.AddRange(testProjects);
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecciona un proyecto de test para migrar:")
                    .PageSize(10)
                    .AddChoices(options));
        }

        public void MigrateFile(string inputFilePath)
        {
            string code = File.ReadAllText(inputFilePath);

            var replacements = new List<(string Pattern, string Replacement)>
        {
            (@"new Mock<(.*?)>\(\)", "Substitute.For<$1>()"),
            (@"Mock<(.*?)>", "$1"),
            (@"\.Setup\(\s*\w+\s*=>\s*\w+\.(.+?)\)", ".$1"),
            (@"\.ReturnsAsync\(([^()]+)\)", ".Returns(Task.FromResult($1))"),
            (@"\.Verifiable\(\)", ""),
            (@"It\.IsAny", "Arg.Any"),
            (@"It\.Is", "Arg.Is"),
            (@"\.Object", ""),
            (@"\busing\s+Moq\b", "using NSubstitute"),
            (@"(?<!\.)\b(\w+)\.Verify\((\w+) => \2(.+?), Times\.(Once(\(\))?|Exactly\((?<times>\d+)\))\)", "$1.Received(${times})$3"),
            (@"(?<!\.)\b(\w+)\.Verify\((\w+) => \2(.+?), Times\.Never\)", "$1.DidNotReceive()$3"),
            (@"(?<!\.)\b(\w+)\.Verify\((\w+) => \2(.+?)\)\)", "$1.Received()$3")

        };

            foreach (var (pattern, replacement) in replacements)
            {
                code = Regex.Replace(code, pattern, replacement);
            }

            File.WriteAllText(inputFilePath, code);
        }
    }
}

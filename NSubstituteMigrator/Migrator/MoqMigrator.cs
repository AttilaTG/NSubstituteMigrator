using NSubstituteMigrator.Migrator.Implementations;

namespace NSubstituteMigrator.Migrator
{
    public class MoqMigrator
    {
        private readonly MigratorService _migratorService;
        public MoqMigrator()
        {
            _migratorService =  new MigratorService(new DirectoryWrapper(), new XmlLoader());
        }
        public void MigrateToNSubstitute() 
        {
            Console.WriteLine("Selecciona el directorio raíz:");
            var rootFolder = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(rootFolder) || !Directory.Exists(rootFolder))
            {
                Console.WriteLine("Directorio inválido.");
                return;
            }

            var testProjects = _migratorService.GetTestProjects(rootFolder);

            if (!testProjects.Any())
            {
                Console.WriteLine("No se encontraron proyectos de test.");
                return;
            }

            var selectedProjectPath = _migratorService.SelectTestProject(testProjects);

            Console.WriteLine($"Has seleccionado: {Path.GetFileNameWithoutExtension(selectedProjectPath)}");

            var testFiles = GetMoqFiles(selectedProjectPath);

            if (!testFiles.Any())
            {
                Console.WriteLine("No se encontraron archivos que usen Moq en este proyecto.");
                return;
            }

            Console.WriteLine($"Archivos detectados que usan Moq: {testFiles.Count()}");

            MigrateFiles(testFiles);

            Console.WriteLine("Migración completada.");
        }

        private void MigrateFiles(IEnumerable<string> testFiles) 
        {
            foreach (var file in testFiles)
            {
                Console.WriteLine($"Migrando: {file}");
                _migratorService.MigrateFile(file);
            }
        }

        private IEnumerable<string> GetMoqFiles(string selectedProjectPath) 
        {
            return Directory.GetFiles(Path.GetDirectoryName(selectedProjectPath)!, "*.cs", SearchOption.AllDirectories)
                                     .Where(file => File.ReadAllText(file).Contains("using Moq"))
                                     .ToList();
        }
    }
}

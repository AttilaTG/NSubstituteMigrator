namespace NSubstituteMigrator.Migrator
{
    public class MoqMigrator
    {
        private readonly MigratorService _migratorService;
        public MoqMigrator()
        {
            _migratorService =  new MigratorService();
        }
        public void MigrateToNSubstitute() 
        {
            try
            {
                Console.WriteLine("Selecciona el directorio que contenga tus proyectos de test:");
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

                if (selectedProjectPath.Equals("All"))
                {
                    foreach (var project in testProjects)
                    {
                        MigrateProject(project);
                    }
                }
                else
                {
                    MigrateProject(selectedProjectPath);
                }

                Console.WriteLine("Migración completada.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return;
            }
        }

        #region private methods
        private void MigrateProject(string selectedProjectPath) 
        {
            var testFiles = GetMoqFiles(selectedProjectPath);

            if (!testFiles.Any())
            {
                Console.WriteLine("No se encontraron archivos que usen Moq en este proyecto.");
                return;
            }

            Console.WriteLine($"Archivos detectados que usan Moq: {testFiles.Count()}");

            MigrateFiles(testFiles);
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
        #endregion
    }
}

using NSubstituteMigrator.Migrator;

class Program
{
    static void Main(string[] args)
    {
        var moqMigrator = new MoqMigrator();
        moqMigrator.MigrateToNSubstitute();
    }
}


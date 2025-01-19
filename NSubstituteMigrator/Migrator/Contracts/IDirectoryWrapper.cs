namespace NSubstituteMigrator.Migrator.Contracts
{
    public interface IDirectoryWrapper
    {
        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
    }
}

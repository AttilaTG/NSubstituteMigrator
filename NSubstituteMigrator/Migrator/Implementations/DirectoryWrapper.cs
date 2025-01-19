using NSubstituteMigrator.Migrator.Contracts;
namespace NSubstituteMigrator.Migrator.Implementations
{
    public class DirectoryWrapper : IDirectoryWrapper
    {
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }
    }
}

using System.Xml.Linq;

namespace NSubstituteMigrator.Migrator.Contracts
{
    public interface IXmlLoader
    {
        XDocument Load(string path);
    }
}

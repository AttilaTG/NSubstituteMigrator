using NSubstituteMigrator.Migrator.Contracts;
using System.Xml.Linq;

namespace NSubstituteMigrator.Migrator.Implementations
{
    public class XmlLoader : IXmlLoader
    {
        public XDocument Load(string path)
        {
            return XDocument.Load(path);
        }
    }
}

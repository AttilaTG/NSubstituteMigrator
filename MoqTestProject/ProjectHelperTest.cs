using Moq;
using NSubstituteMigrator.Migrator;
using NSubstituteMigrator.Migrator.Contracts;
using System.Xml.Linq;

namespace MoqTestProject
{
    public class MigratorServiceTests
    {
        [Fact]
        public void GetTestProjects_ReturnsTestProjects_WhenTestProjectsExist()
        {
            // Arrange
            var rootFolder = "mockRoot";
            var projectFiles = new[] { "mockRoot/project1.csproj", "mockRoot/project2.csproj" };

            var mockDirectoryWrapper = new Mock<IDirectoryWrapper>();
            mockDirectoryWrapper.Setup(d => d.GetFiles(rootFolder, "*.csproj", SearchOption.AllDirectories))
                                .Returns(projectFiles);

            var mockXmlLoader = new Mock<IXmlLoader>();
            mockXmlLoader.Setup(x => x.Load("mockRoot/project1.csproj"))
                         .Returns(new XDocument(new XElement("Project", new XElement("IsTestProject", "true"))));
            mockXmlLoader.Setup(x => x.Load("mockRoot/project2.csproj"))
                         .Returns(new XDocument(new XElement("Project", new XElement("IsTestProject", "false"))));

            var migratorService = new MigratorService(mockDirectoryWrapper.Object, mockXmlLoader.Object);

            // Act
            var testProjects = migratorService.GetTestProjects(rootFolder);

            // Assert
            Assert.Single(testProjects);
            Assert.Contains("mockRoot/project1.csproj", testProjects);
        }

        [Fact]
        public void GetTestProjects_ReturnsEmptyList_WhenNoTestProjectsExist()
        {
            // Arrange
            var rootFolder = "mockRoot";
            var projectFiles = new[] { "mockRoot/project1.csproj", "mockRoot/project2.csproj" };

            var mockDirectoryWrapper = new Mock<IDirectoryWrapper>();
            mockDirectoryWrapper.Setup(d => d.GetFiles(rootFolder, "*.csproj", SearchOption.AllDirectories))
                                .Returns(projectFiles);

            var mockXmlLoader = new Mock<IXmlLoader>();
            mockXmlLoader.Setup(x => x.Load(It.IsAny<string>()))
                         .Returns(new XDocument(new XElement("Project", new XElement("IsTestProject", "false"))));

            var migratorService = new MigratorService(mockDirectoryWrapper.Object, mockXmlLoader.Object);

            // Act
            var testProjects = migratorService.GetTestProjects(rootFolder);

            // Assert
            Assert.Empty(testProjects);
        }

        [Fact]
        public void GetTestProjects_ThrowsException_WhenInvalidXml()
        {
            // Arrange
            var rootFolder = "mockRoot";
            var projectFiles = new[] { "mockRoot/project1.csproj" };

            var mockDirectoryWrapper = new Mock<IDirectoryWrapper>();
            mockDirectoryWrapper.Setup(d => d.GetFiles(rootFolder, "*.csproj", SearchOption.AllDirectories))
                                .Returns(projectFiles);

            var mockXmlLoader = new Mock<IXmlLoader>();
            mockXmlLoader.Setup(x => x.Load(It.IsAny<string>()))
                         .Throws(new Exception("Invalid XML"));

            var migratorService = new MigratorService(mockDirectoryWrapper.Object, mockXmlLoader.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => migratorService.GetTestProjects(rootFolder));
        }

        [Fact]
        public void MigrateFile_PerformsReplacementsCorrectly()
        {
            // Arrange
            var inputFilePath = "mockFile.cs";
            var initialCode = @"
using Moq;

var mock = new Mock<IService>();
mock.Setup(x => x.GetData()).ReturnsAsync(42).Verifiable();
mock.Setup(x => x.Value).Returns(33).Verifiable();
mock.Object.SomeMethod();
";

            var expectedCode = @"
using NSubstitute;

var mock = Substitute.For<IService>();
mock.GetData().Returns(Task.FromResult(42));
mock.Value.Returns(33);
mock.SomeMethod();
";

            var mockDirectoryWrapper = new Mock<IDirectoryWrapper>();
            var mockXmlLoader = new Mock<IXmlLoader>();
            var migratorService = new MigratorService(mockDirectoryWrapper.Object, mockXmlLoader.Object);

            File.WriteAllText(inputFilePath, initialCode); 

            // Act
            migratorService.MigrateFile(inputFilePath);

            // Assert
            var resultingCode = File.ReadAllText(inputFilePath);
            Assert.Equal(expectedCode.Trim(), resultingCode.Trim());
        }

        [Fact]
        public void MigrateFile_DoesNothing_WhenFileIsEmpty()
        {
            // Arrange
            var inputFilePath = "mockFile.cs";
            var initialCode = string.Empty;

            var mockDirectoryWrapper = new Mock<IDirectoryWrapper>();
            var mockXmlLoader = new Mock<IXmlLoader>();
            var migratorService = new MigratorService(mockDirectoryWrapper.Object, mockXmlLoader.Object);

            File.WriteAllText(inputFilePath, initialCode); 

            // Act
            migratorService.MigrateFile(inputFilePath);

            // Assert
            var resultingCode = File.ReadAllText(inputFilePath);
            Assert.Equal(initialCode, resultingCode); 
        }
    }
}
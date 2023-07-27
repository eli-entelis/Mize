using ChainResource;
using ChainResource.Storage;
using Moq;

namespace Tests
{
    public class ChainResourceTests
    {
        [Fact]
        public async Task GetValue_FirstStorageReturnsValue_UsesValueAndDoesNotCallOtherStorages()
        {
            // Arrange
            var value = "Test Value";
            var readOnlyStorageMock1 = CreateReadOnlyStorageMock(value);
            var readOnlyStorageMock2 = CreateReadOnlyStorageMock("Should not be used");
            var readWriteStorageMock = new Mock<IReadAndWriteStorage<string>>();
            var storages = new List<IReadOnlyStorage<string>> { readOnlyStorageMock1.Object, readOnlyStorageMock2.Object, readWriteStorageMock.Object };
            var chainResource = new ChainResource<string>(storages);

            // Act
            var result = await chainResource.GetValue();

            // Assert
            Assert.Equal(value, result);
            readOnlyStorageMock1.Verify(s => s.GetValue(), Times.Once);
            readOnlyStorageMock2.Verify(s => s.GetValue(), Times.Never);
            readWriteStorageMock.Verify(s => s.SetValue(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetValue_AllStoragesFail_ReturnsDefault()
        {
            // Arrange
            var storages = new List<IReadOnlyStorage<string>> { CreateFailedReadOnlyStorageMock().Object };
            var chainResource = new ChainResource<string>(storages);

            // Act
            var result = await chainResource.GetValue();

            // Assert
            Assert.Equal(default(string), result);
        }

        [Fact]
        public async Task GetValue_FirstStorageFails_SecondStorageSucceeds_UsesValueFromSecondStorage()
        {
            // Arrange
            var value = "Test Value";
            var readOnlyStorageMock1 = CreateFailedReadOnlyStorageMock();
            var readOnlyStorageMock2 = CreateReadOnlyStorageMock(value);
            var readWriteStorageMock = new Mock<IReadAndWriteStorage<string>>();
            var storages = new List<IReadOnlyStorage<string>> { readOnlyStorageMock1.Object, readOnlyStorageMock2.Object, readWriteStorageMock.Object };
            var chainResource = new ChainResource<string>(storages);

            // Act
            var result = await chainResource.GetValue();

            // Assert
            Assert.Equal(value, result);
            readOnlyStorageMock1.Verify(s => s.GetValue(), Times.Once);
            readOnlyStorageMock2.Verify(s => s.GetValue(), Times.Once);
            readWriteStorageMock.Verify(s => s.SetValue(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetValue_FirstStorageFails_SecondStorageSucceeds_UsesValueFromSecondStorageAndPropagate()
        {
            // Arrange
            var value = "Test Value";
            var readWriteStorageMock1 = new Mock<IReadAndWriteStorage<string>>();
            var readWriteStorageMock2 = new Mock<IReadAndWriteStorage<string>>();
            var readOnlyStorageMock = CreateReadOnlyStorageMock(value);

            var storages = new List<IReadOnlyStorage<string>> { readWriteStorageMock1.Object, readWriteStorageMock2.Object, readOnlyStorageMock.Object };
            var chainResource = new ChainResource<string>(storages);

            // Act
            var result = await chainResource.GetValue();

            // Assert
            Assert.Equal(value, result);
            readWriteStorageMock1.Verify(s => s.GetValue(), Times.Once);
            readWriteStorageMock1.Verify(s => s.SetValue(It.IsAny<string>()), Times.Once);
            readWriteStorageMock2.Verify(s => s.GetValue(), Times.Once);
            readWriteStorageMock2.Verify(s => s.SetValue(It.IsAny<string>()), Times.Once);
            readOnlyStorageMock.Verify(s => s.GetValue(), Times.Once);
        }

        private Mock<IReadOnlyStorage<string>> CreateReadOnlyStorageMock(string value)
        {
            var storageMock = new Mock<IReadOnlyStorage<string>>();
            storageMock.Setup(s => s.GetValue()).ReturnsAsync(value);
            return storageMock;
        }

        private Mock<IReadOnlyStorage<string>> CreateFailedReadOnlyStorageMock()
        {
            var storageMock = new Mock<IReadOnlyStorage<string>>();
            storageMock.Setup(s => s.GetValue()).ThrowsAsync(new Exception("Storage failed"));
            return storageMock;
        }
    }
}
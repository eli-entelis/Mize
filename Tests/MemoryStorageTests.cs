using ChainResource.Storage;

namespace Tests
{
    public class MemoryStorageTests
    {
        [Fact]
        public async Task GetValue_ValidData_NotExpired_ReturnsValue()
        {
            // Arrange
            var expiration = TimeSpan.FromMinutes(30);
            var value = "Test Value";
            var storage = new MemoryStorage<string>(expiration);
            await storage.SetValue(value);

            // Act
            var result = await storage.GetValue();

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task GetValue_Expired_ReturnsDefault()
        {
            // Arrange
            var expiration = TimeSpan.FromMilliseconds(1);
            var value = "Test Value";
            var storage = new MemoryStorage<string>(expiration);
            await storage.SetValue(value);

            // Wait for the expiration time to pass
            await Task.Delay(1);

            // Act
            var result = await storage.GetValue();

            // Assert
            Assert.Equal(default(string), result);
        }

        [Fact]
        public async Task GetValue_AfterSet_ReturnsValue()
        {
            // Arrange
            var expiration = TimeSpan.FromMinutes(30);
            var value = "Test Value";
            var storage = new MemoryStorage<string>(expiration);

            // Act
            await storage.SetValue(value);
            var result = await storage.GetValue();

            // Assert
            Assert.Equal(value, result);
        }
    }
}
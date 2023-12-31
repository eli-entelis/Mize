﻿using ChainResource.Storage;

namespace Tests
{
    public class FileSystemStorageTests
    {
        [Fact]
        public async Task GetValue_FileExists_ValidData_NotExpired_ReturnsValue()
        {
            // Arrange
            var filePath = "test_file.json";
            var expiration = TimeSpan.FromMinutes(30);
            var value = "Test Value";
            var storage = new FileSystemStorage<string?>(filePath, expiration);
            await storage.SetValue(value);

            // Act
            var result = await storage.GetValue();

            // Assert
            Assert.Equal(value, result);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task GetValue_FileExists_Expired_ReturnsDefault()
        {
            // Arrange
            var filePath = "test_file.json";
            var expiration = TimeSpan.FromMilliseconds(1);
            var value = "Test Value";
            var storage = new FileSystemStorage<string?>(filePath, expiration);
            await storage.SetValue(value);

            // Wait for the expiration time to pass
            await Task.Delay(10);

            // Act
            var result = await storage.GetValue();

            // Assert
            Assert.Equal(default(string), result);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task GetValue_FileDoesNotExist_ReturnsDefault()
        {
            // Arrange
            var filePath = "non_existent_file.json";
            var expiration = TimeSpan.FromMinutes(30);
            var storage = new FileSystemStorage<string>(filePath, expiration);

            // Act
            var result = await storage.GetValue();

            // Assert
            Assert.Equal(default(string), result);
        }

        [Fact]
        public async Task SetValue_ValidData_SetsValueAndExpiration()
        {
            // Arrange
            var filePath = "test_file.json";
            var expiration = TimeSpan.FromMinutes(30);
            var value = "Test Value";
            var storage = new FileSystemStorage<string?>(filePath, expiration);

            // Act
            await storage.SetValue(value);

            // Assert
            var result = await storage.GetValue();
            Assert.Equal(value, result);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public void Dispose_DeletesFile_IfFileExists()
        {
            // Arrange
            string testFilePath = "testfile.txt";
            File.WriteAllText(testFilePath, "Test data");

            // Act
            using (var storage = new FileSystemStorage<string>(testFilePath, TimeSpan.FromSeconds(10)))
            {
                // Call Dispose to delete the test file
                storage.Dispose();
            }

            // Assert
            Assert.False(File.Exists(testFilePath));
        }

        [Fact]
        public void Dispose_DoesNotThrowException_IfFileDoesNotExist()
        {
            // Arrange
            string nonExistentFilePath = "nonexistentfile.txt";

            // Act & Assert
            using (var storage = new FileSystemStorage<string>(nonExistentFilePath, TimeSpan.FromSeconds(10)))
            {
                // Call Dispose; it should not throw any exceptions when the file does not exist.
                storage.Dispose();
            }
        }
    }
}
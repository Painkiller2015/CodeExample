using Microsoft.AspNetCore.Mvc;
using Moq;
using RegLab_Test.Contracts;
using RegLab_Test.Controllers;
using RegLab_Test.Logger;
using RegLab_Test.SettingsService;

namespace UnitTests
{

    [TestClass]
    public class SettingsControllerTests
    {
        private Mock<ISettingsService> _settingsServiceMock;
        private Mock<ILoggerService> _loggerMock;
        private SettingsController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Создаём mock-объекты для зависимостей
            _settingsServiceMock = new Mock<ISettingsService>();
            _loggerMock = new Mock<ILoggerService>();

            // Создаём контроллер с mock-зависимостями
            _controller = new SettingsController(_settingsServiceMock.Object, _loggerMock.Object);
        }

        #region Get Tests

        [TestMethod]
        public async Task Get_ReturnsOkResult_WhenSettingExists()
        {
            // Arrange
            int userId = 1;
            string settingName = "TestSetting";

            var dto = new SettingsDTO { UserId = userId, Name = settingName, CreatedAt = DateTime.Now };
            var setting = new ApiResponse<SettingsDTO>(dto);

            _settingsServiceMock.Setup(s => s.GetAsync(userId, settingName)).ReturnsAsync(setting);

            // Act
            var result = await _controller.Get(userId, settingName);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as ApiResponse<SettingsDTO>;
            Assert.AreEqual(true, response.Success);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenSettingDoesNotExist()
        {
            // Arrange
            int userId = 1;
            string settingName = "NonExistentSetting";
            var setting = new ApiResponse<SettingsDTO>("Setting not found", 404);

            _settingsServiceMock.Setup(s => s.GetAsync(userId, settingName)).ReturnsAsync(setting);

            // Act
            var result = await _controller.Get(userId, settingName);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
        }

        #endregion

        #region Create Tests

        [TestMethod]
        public async Task Create_ReturnsOkResult_WhenSettingIsCreated()
        {
            // Arrange
            var setting = new SettingsDTO() { UserId = 1, Name = "NewSetting", CreatedAt = DateTime.Now };
            var apiResponse = new ApiResponse<SettingsDTO>(setting);

            _settingsServiceMock.Setup(s => s.CreateAsync(setting)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Create(setting);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as ApiResponse<SettingsDTO>;
            Assert.AreEqual(true, response.Success);
        }

        [TestMethod]
        public async Task Create_ReturnsConflict_WhenSettingAlreadyExists()
        {
            // Arrange
            var setting = new SettingsDTO { UserId = 1, Name = "ExistingSetting", CreatedAt = DateTime.Now };
            var apiResponse = new ApiResponse<SettingsDTO>("Setting with the same name already exists.", 409);

            _settingsServiceMock.Setup(s => s.CreateAsync(setting)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Create(setting);

            // Assert
            var conflictResult = result as ConflictObjectResult;
            Assert.IsNotNull(conflictResult);
        }

        #endregion

        #region Update Tests

        [TestMethod]
        public async Task Update_ReturnsOkResult_WhenSettingIsUpdated()
        {
            // Arrange
            int userId = 1;
            string settingName = "ExistingSetting";
            var updateSetting = new SettingsDTO() { UserId = userId, Name = "UpdatedSetting", CreatedAt = DateTime.Now };
            var apiResponse = new ApiResponse<SettingsDTO>(updateSetting);
            _settingsServiceMock.Setup(s => s.UpdateAsync(userId, settingName, updateSetting)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Update(userId, settingName, updateSetting);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as ApiResponse<SettingsDTO>;
            Assert.AreEqual(true, response.Success);
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenSettingToUpdateDoesNotExist()
        {
            // Arrange
            int userId = 1;
            string settingName = "NonExistentSetting";
            var updateSetting = new SettingsDTO { UserId = userId, Name = "UpdatedSetting", CreatedAt = DateTime.Now };
            var apiResponse = new ApiResponse<SettingsDTO>("Setting not found", 404);

            _settingsServiceMock.Setup(s => s.UpdateAsync(userId, settingName, updateSetting)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Update(userId, settingName, updateSetting);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        public async Task Delete_ReturnsOkResult_WhenSettingIsDeleted()
        {
            // Arrange
            int userId = 1;
            string settingName = "SettingToDelete";
            var apiResponse = new ApiResponse<bool>(true);
            _settingsServiceMock.Setup(s => s.DeleteAsync(userId, settingName)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Delete(userId, settingName);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as ApiResponse<bool>;
            Assert.AreEqual(true, response.Data);
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenSettingToDeleteDoesNotExist()
        {
            // Arrange
            int userId = 1;
            string settingName = "NonExistentSettingToDelete";
            var apiResponse = new ApiResponse<bool>("Setting not found", 404);

            _settingsServiceMock.Setup(s => s.DeleteAsync(userId, settingName)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Delete(userId, settingName);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
        }

        #endregion
    }
}


using Microsoft.EntityFrameworkCore;
using RegLab_Test.Logger;
using RegLab_Test.Mongodb.UserSettings.Entity;
using RegLab_Test.MongoDB.DataBase;

namespace RegLab_Test.MongoDB
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DataContext _db;
        private readonly ILoggerService _logger;
        public SettingsRepository(DataContext database, ILoggerService logger)
        {
            _db = database;
            _logger = logger;
        }

        public async Task<UserSettings> GetUserSettingAsync(int userId, string settingName)
        {
            try
            {
                return await _db.UserSettings
                    .FirstOrDefaultAsync(el => el.UserId == userId && el.Name == settingName);
            }
            catch (Exception ex)
            {
                LogError("GetUserSettingAsync", userId, settingName, ex);
                return null;
            }
        }
        public async Task<IEnumerable<UserSettings>> GetAllUserSettingsAsync(int userId)
        {
            try
            {
                return await _db.UserSettings
                   .Where(el => el.UserId == userId)
                   .ToListAsync();
            }
            catch (Exception ex)
            {
                LogError("GetAllUserSettingsAsync", userId, "*", ex);
                return null;
            }
        }

        public async Task<UserSettings> AddNewSettingAsync(UserSettings newSetting)
        {
            try
            {
                var existingSetting = await GetUserSettingAsync(newSetting.UserId, newSetting.Name);
                if (existingSetting != null)
                {
                    return null;
                }

                await _db.UserSettings.AddAsync(newSetting);
                await _db.SaveChangesAsync();
                return newSetting;
            }

            catch (Exception ex)
            {
                LogError("AddNewSettingAsync", newSetting.UserId, newSetting.Name, ex);
                return null;
            }
        }
        public async Task<UserSettings> UpdateUserSettingAsync(int userId, string settingName, UserSettings updatedSetting)
        {
            //TODO запретить переименовать, если уже существует такое имя
            try
            {
                var existingSetting = await GetUserSettingAsync(userId, settingName);

                if (existingSetting == null)
                {
                    return null;
                }

                existingSetting.Update(updatedSetting);
                _db.UserSettings.Update(updatedSetting);
                await _db.SaveChangesAsync();
                return existingSetting;
            }
            catch (Exception ex)
            {
                LogError("UpdateUserSettingAsync", userId, settingName, ex);
                return null;
            }
        }
        public async Task<bool> RemoveUserSettingAsync(int userId, string settingName)
        {
            try
            {
                var setting = await GetUserSettingAsync(userId, settingName);
                if (setting != null)
                {
                    _db.UserSettings.Remove(setting);
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError("RemoveUserSettingAsync", userId, settingName, ex);
                return false;
            }
        }

        public void LogError(string action, int userId, string settingName, Exception ex)
        {
            var errorMessage = $"MongoDB: Error while trying to {action} setting for user {userId} with name {settingName}";
            _logger.LogError(errorMessage, ex);
        }
    }
}

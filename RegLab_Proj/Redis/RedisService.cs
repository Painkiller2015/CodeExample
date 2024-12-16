using Microsoft.Extensions.Options;
using RegLab_Test.Contracts;
using RegLab_Test.Logger;
using StackExchange.Redis;
using System.Text.Json;

namespace RegLab_Test.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _cache;
        private readonly ILoggerService _logger;
        private readonly IOptions<CacheSettings> _cacheOptions;
        public RedisService(IConnectionMultiplexer connectionMultiplexer, IOptions<CacheSettings> cacheSettings, ILoggerService logger)
        {
            _cacheOptions = cacheSettings;
            _cache = connectionMultiplexer.GetDatabase();
            _logger = logger;
        }
        private static string GetUserKey(int userId) => $"user:{userId}:cards";
        public async Task<SettingsDTO> GetUserSettingAsync(int userId, string settingName)
        {
            try
            {
                var settings = await GetAllUserSettingsAsync(userId);
                return settings.Find(setting => setting.Name == settingName);
            }
            catch (Exception ex)
            {
                LogError("GetUserSettingAsync", userId, settingName, ex);
                return null;
            }
        }
        public async Task<List<SettingsDTO>> GetAllUserSettingsAsync(int userId)
        {
            try
            {
                var key = GetUserKey(userId);
                var settingsJson = await _cache.StringGetAsync(key);

                if (settingsJson.IsNullOrEmpty)
                    return new List<SettingsDTO>();

                return JsonSerializer.Deserialize<List<SettingsDTO>>(settingsJson);
            }
            catch (Exception ex)
            {
                LogError("GetAllUserSettings", userId, "*", ex);
                return new List<SettingsDTO>();
            }
        }

        public async Task<SettingsDTO> AddNewSettingAsync(SettingsDTO newSetting)
        {
            try
            {
                var userId = newSetting.UserId;
                var settings = await GetAllUserSettingsAsync(userId);

                if (settings.Any(s => s.Name == newSetting.Name))
                {
                    return null;
                }

                settings.Add(newSetting);
                bool success = await SaveUserSettings(userId, settings);

                return success ? newSetting : null;
            }
            catch (Exception ex)
            {
                LogError("AddNewSettingAsync", newSetting.UserId, newSetting.Name, ex);
                return null;
            }
        }

        public async Task<SettingsDTO> UpdateUserSettingAsync(int userId, string settingName, SettingsDTO newSettings)
        {
            try
            {
                var settings = await GetAllUserSettingsAsync(userId);
                var existingSetting = settings.Find(setting => setting.UserId == userId && setting.Name == newSettings.Name);

                if (existingSetting != null)
                {
                    existingSetting = newSettings.Clone();
                    await SaveUserSettings(userId, settings);
                }
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
                var settings = await GetAllUserSettingsAsync(userId);
                var settingToRemove = settings.FirstOrDefault(s => s.Name == settingName);

                if (settingToRemove == null)
                {
                    return false;
                }

                settings.Remove(settingToRemove);
                return await SaveUserSettings(userId, settings);
            }
            catch (Exception ex)
            {
                LogError("RemoveUserSettingAsync", userId, settingName, ex);
                return false;
            }
        }
        private async Task<bool> SaveUserSettings(int userId, List<SettingsDTO> settings)
        {
            try
            {
                var key = GetUserKey(userId);
                var settingsJson = JsonSerializer.Serialize(settings);
                var cacheDuration = TimeSpan.FromMinutes(_cacheOptions.Value.SettingCacheDuration);

                return await _cache.StringSetAsync(key, settingsJson, cacheDuration);
            }
            catch (Exception ex)
            {
                LogError("SaveUserSettings", userId, "*", ex);
                return false;
            }
        }
        private void LogError(string action, int userId, string settingName, Exception ex)
        {
            _logger.LogError($"Redis: Error while trying to {action} setting for user {userId} with name {settingName}", ex);
        }
    }
}

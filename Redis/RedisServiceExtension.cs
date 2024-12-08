using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Distributed;
using RegLab_Test.Contracts;
using System.Text.Json;

namespace RegLab_Test.Redis
{
    public static class RedisServiceExtension
    {
        //Добавить на обновление кэшей при обновлении сеттинга и удаление из кэша при удалении 
        public static async Task<SettingsDTO> GetSetting(IDistributedCache cache, int userId, string settingName)
        {
            var dto = new SettingsDTO();

            var dtoCollect = await GetAllSettings(cache, userId);
            var dtp = dtoCollect.FirstOrDefault(dto => dto.Name == settingName);

            return dto;
        }
        public static async Task<List<SettingsDTO>> GetAllSettings(IDistributedCache cache, int userId)
        {
            var dto = new List<SettingsDTO>();

            var key = userId.ToString();
            var userString = await cache.GetStringAsync(key);
            if (userString != null) dto = JsonSerializer.Deserialize<List<SettingsDTO>>(userString);

            return dto;
        }

        public static async Task AddNewSetting(IDistributedCache cache, SettingsDTO newSetting)
        {
            var allSettings = await GetAllSettings(cache, newSetting.UserId);
            allSettings.Add(newSetting);

            await cache.SetStringAsync(newSetting.UserId.ToString(), JsonSerializer.Serialize(allSettings));
        }

        public static async Task AddUserSettings(IDistributedCache cache,int userId, List<SettingsDTO> newSettings)
        {
            await cache.SetStringAsync(userId.ToString(), JsonSerializer.Serialize(newSettings));
        }

    }
}

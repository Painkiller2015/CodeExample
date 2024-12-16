using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using RegLab_Test.Contracts;
using RegLab_Test.Mongodb.UserSettings.Entity;
using RegLab_Test.MongoDB;
using RegLab_Test.Redis;
using RegLab_Test.SignalR.Hubs;

namespace RegLab_Test.SettingsService
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _db;
        private readonly IRedisService _cache;
        private readonly IMapper _mapper;
        private readonly IHubContext<SettingsHubService, ISettingsHubService> _hub;
        public SettingsService(ISettingsRepository productRepository, IRedisService cacheService, IMapper mapper, IHubContext<SettingsHubService, ISettingsHubService> hubContext)
        {
            _db = productRepository;
            _cache = cacheService;
            _mapper = mapper;
            _hub = hubContext;
        }
        public async Task<ApiResponse<SettingsDTO>> GetAsync(int userId, string settingName)
        {
            var setting = await _cache.GetUserSettingAsync(userId, settingName);

            if (setting is null)
            {
                var entity = await _db.GetUserSettingAsync(userId, settingName);

                if (entity != null)
                {
                    setting = UserSettingsToDTO(entity);
                    await _cache.AddNewSettingAsync(setting);
                }
            }
            return new ApiResponse<SettingsDTO>(setting);
        }

        public async Task<ApiResponse<IEnumerable<SettingsDTO>>> GetAllAsync(int userId, string sortBy, bool asc)
        {
            var settings = await _cache.GetAllUserSettingsAsync(userId);

            if (settings.Count == 0)
            {
                var entity = await _db.GetAllUserSettingsAsync(userId);

                if (!entity.Any())
                {
                    return new ApiResponse<IEnumerable<SettingsDTO>>(new List<SettingsDTO>());
                }

                foreach (var item in entity)
                {
                    var setting = _mapper.Map<SettingsDTO>(item);

                    settings.Add(setting);
                    await _cache.AddNewSettingAsync(setting);
                }
            }
            SortSettings(settings, sortBy, asc);
            return new ApiResponse<IEnumerable<SettingsDTO>>(settings);
        }

        public async Task<ApiResponse<SettingsDTO>> CreateAsync(SettingsDTO document)
        {
            var userId = document.UserId;
            var settingName = document.Name;

            document.CreatedAt = DateTime.UtcNow;

            var existingSetting = await _cache.GetUserSettingAsync(userId, settingName);
            if (existingSetting != null)
            {
                return new ApiResponse<SettingsDTO>("A configuration with this name already exists.", 400);
            }

            var settingEntity = await _db.GetUserSettingAsync(userId, settingName);
            if (settingEntity != null)
            {
                return new ApiResponse<SettingsDTO>("A configuration with this name already exists.", 400);
            }

            settingEntity = DTOToUserSettings(document);

            await _db.AddNewSettingAsync(settingEntity);
            await _cache.AddNewSettingAsync(document);

            SendNotify(userId, NotifyType.add);
            return new ApiResponse<SettingsDTO>(document);
        }

        public async Task<ApiResponse<SettingsDTO>> UpdateAsync(int userId, string settingsName, SettingsDTO updateSetting)
        {
            var cachedSetting = await _cache.GetUserSettingAsync(userId, updateSetting.Name);
            if (cachedSetting != null && cachedSetting.Name != settingsName)
            {
                return new ApiResponse<SettingsDTO>("A configuration with the same name already exists in the cache.", 409);
            }

            var existingSettingWithName = await _db.GetUserSettingAsync(userId, updateSetting.Name);
            if (existingSettingWithName != null && existingSettingWithName.Name != settingsName)
            {
                return new ApiResponse<SettingsDTO>("A configuration with the same name already exists.", 409);
            }

            var userSetting = DTOToUserSettings(updateSetting);  
            var updatedSetting = await _db.UpdateUserSettingAsync(userId, settingsName, userSetting);

            if (updatedSetting == null)
            {
                return null;
            }

            await _cache.UpdateUserSettingAsync(userId, settingsName, updateSetting);
            
            SendNotify(userId, NotifyType.update);
            return new ApiResponse<SettingsDTO>(updateSetting);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int userId, string settingsName)
        {
            var setting = await _cache.GetUserSettingAsync(userId, settingsName);
            if (setting == null)
            {
                var settingDB = await _db.GetUserSettingAsync(userId, settingsName);
                if (settingDB == null)
                {
                    return new ApiResponse<bool>("Configuration not found.", 404);  
                }
            }

            var isDel = await _db.RemoveUserSettingAsync(userId, settingsName);
            await _cache.RemoveUserSettingAsync(userId, settingsName);

            SendNotify(userId, NotifyType.delete);
            return new ApiResponse<bool>(isDel);
        }

        public void SortSettings(List<SettingsDTO> settings, string sortBy, bool asc)
        {
            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                settings = asc
                    ? settings.OrderBy(s => s.Name).ToList()
                    : settings.OrderByDescending(s => s.Name).ToList();
            }
            else if (sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
            {
                settings = asc
                    ? settings.OrderBy(s => s.CreatedAt).ToList()
                    : settings.OrderByDescending(s => s.CreatedAt).ToList();
            }

        }
        private IEnumerable<SettingsDTO> UserSettingsListToDTO(IEnumerable<UserSettings> settings)
        {
            return settings.Select(el => _mapper.Map<SettingsDTO>(el));
        }
        private IEnumerable<UserSettings> DTOListToUserSettings(IEnumerable<SettingsDTO> settings)
        {
            return settings.Select(el => _mapper.Map<UserSettings>(el));
        }
        private SettingsDTO UserSettingsToDTO(UserSettings settings)
        {
            return _mapper.Map<SettingsDTO>(settings);
        }
        private UserSettings DTOToUserSettings(SettingsDTO settings)
        {
            return _mapper.Map<UserSettings>(settings);
        }
        private async void SendNotify(int userId, NotifyType type)
        {
            var settings = await _db.GetAllUserSettingsAsync(userId);
            var settingsDTO = UserSettingsListToDTO(settings);

            switch (type)
            {
                case NotifyType.add:
                    await SettingsHubServiceExtensions.NotifySettingAdded(_hub, settingsDTO);
                    break;
                case NotifyType.update:
                    await SettingsHubServiceExtensions.NotifySettingUpdated(_hub, settingsDTO);
                    break;
                case NotifyType.delete:
                    await SettingsHubServiceExtensions.NotifySettingDeleted(_hub, settingsDTO);
                    break;
                default:
                    break;

            }
        }
    }
}

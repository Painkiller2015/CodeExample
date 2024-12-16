using RegLab_Test.Contracts;

namespace RegLab_Test.SettingsService
{
    public interface ISettingsService
    {
        Task<ApiResponse<SettingsDTO>> CreateAsync(SettingsDTO document);
        Task<ApiResponse<bool>> DeleteAsync(int userId, string settingsName);
        Task<ApiResponse<SettingsDTO>> GetAsync(int userId, string settingName);
        Task<ApiResponse<IEnumerable<SettingsDTO>>> GetAllAsync(int userId, string sortBy, bool asc);
        Task<ApiResponse<SettingsDTO>> UpdateAsync(int userId, string settingsName, SettingsDTO updateSetting);
    }
}

using RegLab_Test.Contracts;

namespace RegLab_Test.SignalR.Hubs
{
    public interface ISettingsHubService
    {
        Task NotifySettingAdded(string message, IEnumerable<SettingsDTO> settings);
        Task NotifySettingUpdated(string message, IEnumerable<SettingsDTO> settings);
        Task NotifySettingDeleted(string message, IEnumerable<SettingsDTO> settings);
    }
}
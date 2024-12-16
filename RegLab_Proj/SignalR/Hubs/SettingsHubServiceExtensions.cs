using Microsoft.AspNetCore.SignalR;
using RegLab_Test.Contracts;

namespace RegLab_Test.SignalR.Hubs
{
    public static class SettingsHubServiceExtensions
    {

        public static async Task NotifySettingAdded(IHubContext<SettingsHubService, ISettingsHubService> hubContext, IEnumerable<SettingsDTO> settings)
        {
            await hubContext.Clients.All.NotifySettingAdded("A new configuration has been added.", settings);
        }

        public static async Task NotifySettingUpdated(IHubContext<SettingsHubService, ISettingsHubService> hubContext, IEnumerable<SettingsDTO> settings)
        {
            await hubContext.Clients.All.NotifySettingUpdated("A configuration has been updated.", settings);
        }

        public static async Task NotifySettingDeleted(IHubContext<SettingsHubService, ISettingsHubService> hubContext, IEnumerable<SettingsDTO> settings)
        {
            await hubContext.Clients.All.NotifySettingDeleted("A configuration has been deleted.", settings);
        }
    }
}

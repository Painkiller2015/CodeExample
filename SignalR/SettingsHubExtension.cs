using Microsoft.AspNetCore.SignalR;

namespace RegLab_Test.SignalR
{
    public static class SettingsHubExtension
    {
        public static async Task SendCustomMessage(IHubContext<SettingsHub, ISettingsHub> hubContext, string message)
        {
            await hubContext.Clients.All.SendCustomMessage("getCustomMessage", message);
        }
        public static async Task SendSettingsAddNotification(IHubContext<SettingsHub, ISettingsHub> hubContext)
        {
            await hubContext.Clients.All.SendSettingsAddNotification("SettingAdded", "Конфигурация добавлена");
        }
        public static async Task SendSettingsUpdateNotification(IHubContext<SettingsHub, ISettingsHub> hubContext)
        {
            await hubContext.Clients.All.SendSettingsUpdateNotification("SettingUpdated", "Конфигурация обновлена");
        }
        public static async Task SendSettingsDeleteNotification(IHubContext<SettingsHub, ISettingsHub> hubContext)
        {
            await hubContext.Clients.All.SendSettingsDeleteNotification("SettingDeleted", "Конфигурация удалена");
        }
    }
}

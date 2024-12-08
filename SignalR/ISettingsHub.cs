namespace RegLab_Test.SignalR
{
    public interface ISettingsHub
    {
        public Task SendCustomMessage(string name, string message);
        public Task SendSettingsAddNotification(string name, string message);
        public Task SendSettingsUpdateNotification(string name, string message);
        public Task SendSettingsDeleteNotification(string name, string message);
    }
}

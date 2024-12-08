using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RegLab_Test.SignalR
{
    [Authorize]
    public class SettingsHub : Hub<ISettingsHub>
    {

    }
}

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TES_Learning_App.API.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time synchronization across all admin users
    /// Broadcasts changes to activities, exercises, and other entities
    /// </summary>
    public class AdminHub : Hub
    {
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminUsers");
            await Clients.Caller.SendAsync("Joined", "Successfully connected to admin group");
        }

        public async Task LeaveAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminUsers");
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminUsers");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminUsers");
            await base.OnDisconnectedAsync(exception);
        }
    }
}





































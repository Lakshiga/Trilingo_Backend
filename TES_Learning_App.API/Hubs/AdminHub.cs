using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace TES_Learning_App.API.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time synchronization across all admin users.
    /// Secured: Only Admins can connect.
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")] 
    public class AdminHub : Hub
    {
        // Connection open aagum bothe Automatic-a "AdminUsers" group la add aagidum.
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminUsers");
            
            // Optional: Send a welcome message to the frontend debug logs
            await Clients.Caller.SendAsync("Joined", "Successfully secured connection to admin group");
            
            await base.OnConnectedAsync();
        }

        // Connection cut aagum bothu automatic-a remove aagidum.
        // 🛠️ BUG FIX: Added '?' to Exception to fix the build warning
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminUsers");
            await base.OnDisconnectedAsync(exception);
        }
    }
}














































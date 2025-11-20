using Microsoft.AspNetCore.SignalR;
using TES_Learning_App.API.Hubs;

namespace TES_Learning_App.API.Services
{
    /// <summary>
    /// Service to broadcast real-time updates to all connected admin clients
    /// </summary>
    public interface IRealtimeBroadcastService
    {
        Task BroadcastActivityCreatedAsync(object activity);
        Task BroadcastActivityUpdatedAsync(object activity);
        Task BroadcastActivityDeletedAsync(int activityId);
        Task BroadcastExerciseCreatedAsync(object exercise);
        Task BroadcastExerciseUpdatedAsync(object exercise);
        Task BroadcastExerciseDeletedAsync(int exerciseId);
    }

    public class RealtimeBroadcastService : IRealtimeBroadcastService
    {
        private readonly IHubContext<AdminHub> _hubContext;

        public RealtimeBroadcastService(IHubContext<AdminHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastActivityCreatedAsync(object activity)
        {
            await _hubContext.Clients.Group("AdminUsers").SendAsync("ActivityCreated", activity);
        }

        public async Task BroadcastActivityUpdatedAsync(object activity)
        {
            await _hubContext.Clients.Group("AdminUsers").SendAsync("ActivityUpdated", activity);
        }

        public async Task BroadcastActivityDeletedAsync(int activityId)
        {
            await _hubContext.Clients.Group("AdminUsers").SendAsync("ActivityDeleted", activityId);
        }

        public async Task BroadcastExerciseCreatedAsync(object exercise)
        {
            await _hubContext.Clients.Group("AdminUsers").SendAsync("ExerciseCreated", exercise);
        }

        public async Task BroadcastExerciseUpdatedAsync(object exercise)
        {
            await _hubContext.Clients.Group("AdminUsers").SendAsync("ExerciseUpdated", exercise);
        }

        public async Task BroadcastExerciseDeletedAsync(int exerciseId)
        {
            await _hubContext.Clients.Group("AdminUsers").SendAsync("ExerciseDeleted", exerciseId);
        }
    }
}





































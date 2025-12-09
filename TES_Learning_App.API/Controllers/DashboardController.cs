using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Infrastructure.Data;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public DashboardController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet("activity-type-statistics")]
        public async Task<IActionResult> GetActivityTypeStatistics()
        {
            try
            {
                // Get all activity types with their main activities
                var activityTypes = await _context.Set<ActivityType>()
                    .Include(at => at.MainActivity)
                    .ToListAsync();

                // Group by MainActivity and count activity types
                var statistics = activityTypes
                    .GroupBy(at => new
                    {
                        MainActivityId = at.MainActivityId,
                        MainActivityName = at.MainActivity != null 
                            ? (at.MainActivity.Name_en ?? at.MainActivity.Name_ta ?? at.MainActivity.Name_si ?? $"Main Activity {at.MainActivityId}")
                            : $"Main Activity {at.MainActivityId}"
                    })
                    .Select(g => new
                    {
                        mainActivityId = g.Key.MainActivityId,
                        mainActivityName = g.Key.MainActivityName,
                        activityTypeCount = g.Count() // Count of activity types per main activity
                    })
                    .OrderBy(s => s.mainActivityName)
                    .ToList();

                // Ensure we only return unique main activities (one entry per main activity)
                // Group by mainActivityId to ensure uniqueness
                var uniqueStatistics = statistics
                    .GroupBy(s => s.mainActivityId)
                    .Select(g => new
                    {
                        mainActivityId = g.Key,
                        mainActivityName = g.First().mainActivityName,
                        activityTypeCount = g.Sum(x => x.activityTypeCount) // Sum counts if somehow duplicated
                    })
                    .OrderBy(s => s.mainActivityName)
                    .ToList();

                return Ok(uniqueStatistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error fetching activity type statistics: {ex.Message}" });
            }
        }

        [HttpGet("user-statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                // Get all users with their roles
                var users = await _context.Set<User>()
                    .Include(u => u.Role)
                    .ToListAsync();

                // Group by role and count
                var statistics = users
                    .GroupBy(u => new { 
                        RoleId = u.RoleId, 
                        RoleName = u.Role != null ? (u.Role.RoleName ?? "Unknown") : "Unknown"
                    })
                    .Select(g => new
                    {
                        roleId = g.Key.RoleId,
                        roleName = g.Key.RoleName,
                        count = g.Count()
                    })
                    .OrderByDescending(s => s.count)
                    .ToList();

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error fetching user statistics: {ex.Message}" });
            }
        }
    }
}


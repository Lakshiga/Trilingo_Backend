using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Progress.Requests;
using TES_Learning_App.Application_Layer.DTOs.Progress.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Application_Layer.Exceptions;

namespace TES_Learning_App.Application_Layer.Services
{
    public class StudentProgressService : IStudentProgressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentProgressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProgressDto> CreateProgressAsync(CreateProgressDto dto, Guid parentUserId)
        {
            // Validate student ownership
            if (!await ValidateStudentOwnershipAsync(dto.StudentId, parentUserId))
            {
                throw new UnauthorizedAccessException("Access denied. Student does not belong to this parent.");
            }

            // Validate activity exists
            var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(dto.ActivityId);
            if (activity == null)
            {
                throw new KeyNotFoundException("Activity not found.");
            }

            // Enforce “first attempt wins” – once a score is recorded for a student+activity, it cannot be changed
            var existing = await _unitOfWork.StudentProgressRepository
                .GetLatestByStudentAndActivityAsync(dto.StudentId, dto.ActivityId);
            if (existing != null)
            {
                throw new ValidationException("Score", new[]
                {
                    "A score is already recorded for this exercise. It cannot be changed or overwritten."
                });
            }

            // Validate score
            if (dto.Score < 0 || dto.Score > dto.MaxScore)
            {
                throw new ValidationException("Score", new[] { $"Score must be between 0 and {dto.MaxScore}." });
            }

            var progress = new StudentProgress
            {
                StudentId = dto.StudentId,
                ActivityId = dto.ActivityId,
                Score = dto.Score,
                MaxScore = dto.MaxScore,
                TimeSpentSeconds = dto.TimeSpentSeconds,
                AttemptNumber = 1, // first attempt is the only attempt we persist
                IsCompleted = dto.IsCompleted,
                Notes = dto.Notes,
                CompletedAt = DateTime.UtcNow
            };

            await _unitOfWork.StudentProgressRepository.AddAsync(progress);
            await _unitOfWork.CompleteAsync();

            return await MapToProgressDtoAsync(progress);
        }

        public async Task<ProgressDto?> GetProgressByIdAsync(int id, Guid? parentUserId = null)
        {
            var progress = await _unitOfWork.StudentProgressRepository.GetByIdAsync(id);
            if (progress == null) return null;

            // If parentUserId provided, validate ownership
            if (parentUserId.HasValue && progress.StudentId.HasValue)
            {
                if (!await ValidateStudentOwnershipAsync(progress.StudentId.Value, parentUserId.Value))
                {
                    throw new UnauthorizedAccessException("Access denied.");
                }
            }

            return await MapToProgressDtoAsync(progress);
        }

        public async Task<PagedProgressResponse> GetFilteredProgressAsync(ProgressFilterDto filter, Guid? parentUserId = null)
        {
            // If parentUserId provided, ensure they can only see their children's progress
            if (parentUserId.HasValue && filter.StudentId.HasValue)
            {
                if (!await ValidateStudentOwnershipAsync(filter.StudentId.Value, parentUserId.Value))
                {
                    throw new UnauthorizedAccessException("Access denied.");
                }
            }
            else if (parentUserId.HasValue)
            {
                // If no studentId filter, get all students for this parent and filter
                var students = await _unitOfWork.StudentRepository.GetStudentsByParentIdAsync(parentUserId.Value);
                var studentIds = students.Select(s => s.Id).ToList();
                // Note: This requires repository method enhancement or filtering in service
            }

            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var progressList = await _unitOfWork.StudentProgressRepository.GetFilteredAsync(
                studentId: filter.StudentId,
                activityId: filter.ActivityId,
                stageId: filter.StageId,
                levelId: filter.LevelId,
                startDate: filter.StartDate,
                endDate: filter.EndDate,
                minScore: filter.MinScore,
                maxScore: filter.MaxScore,
                isCompleted: filter.IsCompleted,
                skip: skip,
                take: filter.PageSize,
                sortBy: filter.SortBy,
                sortDescending: filter.SortDescending
            );

            var totalCount = await _unitOfWork.StudentProgressRepository.GetCountAsync(
                studentId: filter.StudentId,
                activityId: filter.ActivityId,
                stageId: filter.StageId,
                levelId: filter.LevelId,
                startDate: filter.StartDate,
                endDate: filter.EndDate,
                minScore: filter.MinScore,
                maxScore: filter.MaxScore,
                isCompleted: filter.IsCompleted
            );

            var progressDtos = new List<ProgressDto>();
            foreach (var progress in progressList)
            {
                progressDtos.Add(await MapToProgressDtoAsync(progress));
            }

            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            return new PagedProgressResponse
            {
                Data = progressDtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = filter.PageNumber > 1,
                HasNextPage = filter.PageNumber < totalPages
            };
        }

        public async Task<IEnumerable<ProgressDto>> GetStudentProgressAsync(Guid studentId, Guid? parentUserId = null)
        {
            // Validate ownership if parentUserId provided
            if (parentUserId.HasValue)
            {
                if (!await ValidateStudentOwnershipAsync(studentId, parentUserId.Value))
                {
                    throw new UnauthorizedAccessException("Access denied.");
                }
            }

            var progressList = await _unitOfWork.StudentProgressRepository.GetByStudentIdAsync(studentId);
            var result = new List<ProgressDto>();

            foreach (var progress in progressList)
            {
                result.Add(await MapToProgressDtoAsync(progress));
            }

            return result;
        }

        public async Task<ProgressSummaryDto> GetStudentSummaryAsync(Guid studentId, Guid? parentUserId = null)
        {
            // Validate ownership if parentUserId provided
            if (parentUserId.HasValue)
            {
                if (!await ValidateStudentOwnershipAsync(studentId, parentUserId.Value))
                {
                    throw new UnauthorizedAccessException("Access denied.");
                }
            }

            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            if (student == null || student.IsDeleted)
            {
                throw new KeyNotFoundException("Student not found.");
            }

            var allProgress = await _unitOfWork.StudentProgressRepository.GetByStudentIdAsync(studentId);
            var completedProgress = allProgress.Where(p => p.IsCompleted).ToList();

            var totalCompleted = completedProgress.Count;
            var totalAttempted = allProgress.Select(p => p.ActivityId).Distinct().Count();
            var averageScore = allProgress.Any() && allProgress.All(p => p.MaxScore > 0)
                ? allProgress.Average(p => (double)p.Score / p.MaxScore * 100)
                : 0;
            var totalTimeSpent = allProgress.Sum(p => p.TimeSpentSeconds);
            var lastActivityDate = allProgress.OrderByDescending(p => p.CompletedAt).FirstOrDefault()?.CompletedAt;

            var recentActivities = allProgress
                .OrderByDescending(p => p.CompletedAt)
                .Take(5)
                .Select(p => new RecentActivityDto
                {
                    ActivityId = p.ActivityId,
                    ActivityName = p.Activity?.Name_en ?? "Unknown",
                    Score = p.Score,
                    MaxScore = p.MaxScore,
                    CompletedAt = p.CompletedAt
                })
                .ToList();

            return new ProgressSummaryDto
            {
                StudentId = studentId,
                StudentNickname = student.Nickname,
                StudentAvatar = student.Avatar,
                TotalActivitiesCompleted = totalCompleted,
                TotalActivitiesAttempted = totalAttempted,
                AverageScore = Math.Round(averageScore, 2),
                TotalXpPoints = student.XpPoints,
                TotalTimeSpentSeconds = totalTimeSpent,
                TotalTimeSpentFormatted = FormatTime(totalTimeSpent),
                LastActivityDate = lastActivityDate,
                RecentActivities = recentActivities
            };
        }

        public async Task<StudentStatsDto> GetStudentStatsAsync(Guid studentId, Guid? parentUserId = null)
        {
            // Validate ownership if parentUserId provided
            if (parentUserId.HasValue)
            {
                if (!await ValidateStudentOwnershipAsync(studentId, parentUserId.Value))
                {
                    throw new UnauthorizedAccessException("Access denied.");
                }
            }

            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            if (student == null || student.IsDeleted)
            {
                throw new KeyNotFoundException("Student not found.");
            }

            var allProgress = await _unitOfWork.StudentProgressRepository.GetByStudentIdAsync(studentId);
            var completedProgress = allProgress.Where(p => p.IsCompleted && p.MaxScore > 0).ToList();

            var totalCompleted = completedProgress.Count;
            var totalAttempted = allProgress.Select(p => p.ActivityId).Distinct().Count();
            var averageScore = completedProgress.Any()
                ? completedProgress.Average(p => (double)p.Score / p.MaxScore * 100)
                : 0;
            var bestScore = completedProgress.Any()
                ? completedProgress.Max(p => (double)p.Score / p.MaxScore * 100)
                : 0;
            var worstScore = completedProgress.Any()
                ? completedProgress.Min(p => (double)p.Score / p.MaxScore * 100)
                : 0;
            var totalTimeSpent = allProgress.Sum(p => p.TimeSpentSeconds);

            // Group by stage
            var activitiesByStage = allProgress
                .Where(p => p.Activity?.Stage != null)
                .GroupBy(p => p.Activity.Stage.Name_en ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            var averageScoreByStage = allProgress
                .Where(p => p.Activity?.Stage != null && p.IsCompleted && p.MaxScore > 0)
                .GroupBy(p => p.Activity.Stage.Name_en ?? "Unknown")
                .ToDictionary(
                    g => g.Key,
                    g => Math.Round(g.Average(p => (double)p.Score / p.MaxScore * 100), 2)
                );

            var recentProgress = allProgress
                .OrderByDescending(p => p.CompletedAt)
                .Take(10)
                .Select(p => new ActivityProgressDto
                {
                    ActivityId = p.ActivityId,
                    ActivityName = p.Activity?.Name_en ?? "Unknown",
                    Score = p.Score,
                    MaxScore = p.MaxScore,
                    CompletedAt = p.CompletedAt,
                    AttemptNumber = p.AttemptNumber
                })
                .ToList();

            return new StudentStatsDto
            {
                StudentId = studentId,
                StudentNickname = student.Nickname,
                TotalCompleted = totalCompleted,
                TotalAttempted = totalAttempted,
                AverageScore = Math.Round(averageScore, 2),
                BestScore = Math.Round(bestScore, 2),
                WorstScore = Math.Round(worstScore, 2),
                TotalXpPoints = student.XpPoints,
                TotalTimeSpentSeconds = totalTimeSpent,
                ActivitiesByStage = activitiesByStage,
                AverageScoreByStage = averageScoreByStage,
                RecentProgress = recentProgress
            };
        }

        public async Task<ProgressDto> UpdateProgressAsync(int id, UpdateProgressDto dto, Guid? parentUserId = null)
        {
            var progress = await _unitOfWork.StudentProgressRepository.GetByIdAsync(id);
            if (progress == null)
            {
                throw new KeyNotFoundException("Progress record not found.");
            }

            // Validate ownership if parentUserId provided
            if (parentUserId.HasValue && progress.StudentId.HasValue)
            {
                if (!await ValidateStudentOwnershipAsync(progress.StudentId.Value, parentUserId.Value))
                {
                    throw new UnauthorizedAccessException("Access denied.");
                }
            }

            // Score for a student+exercise is immutable after first attempt
            if (dto.Score.HasValue || dto.MaxScore.HasValue || dto.AttemptNumber.HasValue)
            {
                throw new ValidationException("Score", new[]
                {
                    "Score/attempt are locked after the first attempt. CreateProgress records the only allowed score."
                });
            }

            // Update only provided mutable fields
            if (dto.TimeSpentSeconds.HasValue)
                progress.TimeSpentSeconds = dto.TimeSpentSeconds.Value;

            if (dto.IsCompleted.HasValue)
                progress.IsCompleted = dto.IsCompleted.Value;

            if (dto.Notes != null)
                progress.Notes = dto.Notes;

            await _unitOfWork.StudentProgressRepository.UpdateAsync(progress);
            await _unitOfWork.CompleteAsync();

            return await MapToProgressDtoAsync(progress);
        }

        public async Task<bool> DeleteProgressAsync(int id, Guid? parentUserId = null)
        {
            var progress = await _unitOfWork.StudentProgressRepository.GetByIdAsync(id);
            if (progress == null)
            {
                return false;
            }

            // Validate ownership if parentUserId provided
            if (parentUserId.HasValue && progress.StudentId.HasValue)
            {
                if (!await ValidateStudentOwnershipAsync(progress.StudentId.Value, parentUserId.Value))
                {
                    throw new UnauthorizedAccessException("Access denied.");
                }
            }

            await _unitOfWork.StudentProgressRepository.DeleteAsync(progress);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ValidateStudentOwnershipAsync(Guid studentId, Guid parentUserId)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            return student != null && !student.IsDeleted && student.ParentId == parentUserId;
        }

        private Task<ProgressDto> MapToProgressDtoAsync(StudentProgress progress)
        {
            var percentageScore = progress.MaxScore > 0
                ? Math.Round((double)progress.Score / progress.MaxScore * 100, 2)
                : 0;

            return Task.FromResult(new ProgressDto
            {
                Id = progress.Id,
                StudentId = progress.StudentId,
                StudentNickname = progress.Student?.Nickname,
                StudentAvatar = progress.Student?.Avatar,
                ActivityId = progress.ActivityId,
                ActivityName = progress.Activity?.Name_en,
                ActivityNameEn = progress.Activity?.Name_en,
                ActivityNameTa = progress.Activity?.Name_ta,
                ActivityNameSi = progress.Activity?.Name_si,
                StageId = progress.Activity?.StageId,
                StageName = progress.Activity?.Stage?.Name_en,
                LevelId = progress.Activity?.Stage?.LevelId,
                LevelName = progress.Activity?.Stage?.Level?.Name_en,
                Score = progress.Score,
                MaxScore = progress.MaxScore,
                PercentageScore = percentageScore,
                CompletedAt = progress.CompletedAt,
                TimeSpentSeconds = progress.TimeSpentSeconds,
                TimeSpentFormatted = FormatTime(progress.TimeSpentSeconds),
                AttemptNumber = progress.AttemptNumber,
                IsCompleted = progress.IsCompleted,
                Notes = progress.Notes
            });
        }

        private string FormatTime(int seconds)
        {
            if (seconds < 60)
                return $"{seconds}s";

            var minutes = seconds / 60;
            var remainingSeconds = seconds % 60;

            if (minutes < 60)
                return remainingSeconds > 0 ? $"{minutes}m {remainingSeconds}s" : $"{minutes}m";

            var hours = minutes / 60;
            var remainingMinutes = minutes % 60;

            return remainingMinutes > 0 ? $"{hours}h {remainingMinutes}m" : $"{hours}h";
        }
    }
}



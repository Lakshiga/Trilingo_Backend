using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Student.Request;
using TES_Learning_App.Application_Layer.DTOs.Student.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class StudentProgressService : IStudentProgressService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public StudentProgressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<ExerciseResultResponseDto> SubmitExerciseResultAsync(Guid studentId, SubmitExerciseResultDto dto)
        {
            // Check if student has already completed this exercise
            var existingProgress = await _unitOfWork.StudentProgressRepository
                .FindAsync(sp => sp.StudentId == studentId && sp.ExerciseId == dto.ExerciseId);
            
            var isFirstAttempt = !existingProgress.Any();
            var pointsEarned = 0;
            
            // Get the student to update XP points
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new Exception("Student not found.");
                
            // Get the exercise to get the activity ID
            var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(dto.ExerciseId);
            if (exercise == null)
                throw new Exception("Exercise not found.");
            
            if (isFirstAttempt)
            {
                // First attempt - calculate points based on score (out of 10)
                // Assuming score is a percentage (0-100), convert to points (0-10)
                pointsEarned = (int)Math.Round((double)dto.Score / 100 * 10);
                
                // Save the progress
                var studentProgress = new StudentProgress
                {
                    StudentId = studentId,
                    ActivityId = exercise.ActivityId,
                    ExerciseId = dto.ExerciseId,
                    Score = dto.Score,
                    CompletedAt = DateTime.UtcNow
                };
                
                await _unitOfWork.StudentProgressRepository.AddAsync(studentProgress);
                
                // Award points based on performance
                student.XpPoints += pointsEarned;
                
                await _unitOfWork.StudentRepository.UpdateAsync(student);
            }
            else
            {
                // Not the first attempt - just update the score without awarding points
                var progress = existingProgress.First();
                progress.Score = dto.Score;
                progress.CompletedAt = DateTime.UtcNow;
                
                await _unitOfWork.StudentProgressRepository.UpdateAsync(progress);
            }
            
            await _unitOfWork.CompleteAsync();
            
            return new ExerciseResultResponseDto
            {
                PointsEarned = pointsEarned,
                IsFirstAttempt = isFirstAttempt,
                TotalXpPoints = student.XpPoints
            };
        }
        
        // மாணவர் செயல்திறன் சுருக்கத்தைப் பெறும் செயல்பாடு
        public async Task<ProgressSummaryDto> GetStudentProgressSummaryAsync(Guid studentId)
        {
            // Get student details
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new Exception("Student not found.");
            
            // Get all progress records for this student
            var progressRecords = await _unitOfWork.StudentProgressRepository
                .FindAsync(sp => sp.StudentId == studentId);
            
            var progressList = progressRecords.ToList();
            
            // Calculate summary statistics
            var totalActivitiesCompleted = progressList.Count;
            var totalActivitiesAttempted = progressList.Count; // For now, assuming all are completed
            var averageScore = totalActivitiesCompleted > 0 
                ? progressList.Average(p => p.Score) 
                : 0;
            
            // Get recent activities (last 5)
            var recentActivities = new List<RecentActivityDto>();
            var recentProgress = progressList
                .OrderByDescending(p => p.CompletedAt)
                .Take(5)
                .ToList();
            
            foreach (var progress in recentProgress)
            {
                var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(progress.ActivityId);
                recentActivities.Add(new RecentActivityDto
                {
                    ActivityId = progress.ActivityId,
                    ActivityName = activity?.Name_en ?? "Unknown Activity",
                    Score = progress.Score,
                    MaxScore = 100, // Assuming max score is 100
                    CompletedAt = progress.CompletedAt
                });
            }
            
            // Get last activity date
            var lastActivityDate = progressList.Any() 
                ? progressList.Max(p => p.CompletedAt) 
                : (DateTime?)null;
            
            return new ProgressSummaryDto
            {
                StudentId = student.Id,
                StudentNickname = student.Nickname,
                StudentAvatar = student.Avatar,
                TotalActivitiesCompleted = totalActivitiesCompleted,
                TotalActivitiesAttempted = totalActivitiesAttempted,
                AverageScore = Math.Round(averageScore, 2),
                TotalXpPoints = student.XpPoints,
                TotalTimeSpentSeconds = 0, // This would need to be tracked separately
                TotalTimeSpentFormatted = "0h 0m", // This would need to be calculated
                LastActivityDate = lastActivityDate,
                RecentActivities = recentActivities
            };
        }
    }
}
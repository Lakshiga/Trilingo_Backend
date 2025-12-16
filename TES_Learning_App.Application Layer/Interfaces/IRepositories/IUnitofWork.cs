using System;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Interfaces.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Student> StudentRepository { get; }
        IGenericRepository<Activity> ActivityRepository { get; }
        IGenericRepository<Exercise> ExerciseRepository { get; }
        IGenericRepository<StudentProgress> StudentProgressRepository { get; }
        IGenericRepository<Level> LevelRepository { get; }
        IGenericRepository<Stage> StageRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<ActivityType> ActivityTypeRepository { get; }
        IGenericRepository<MainActivity> MainActivityRepository { get; }
        
        // New repository for ExerciseAttempt
        IGenericRepository<ExerciseAttempt> ExerciseAttemptRepository { get; }

        // Generic repository accessor
        IGenericRepository<T> Repository<T>() where T : class;

        Task<int> CompleteAsync();
    }
}
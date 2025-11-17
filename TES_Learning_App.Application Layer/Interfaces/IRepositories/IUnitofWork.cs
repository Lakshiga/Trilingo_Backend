using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Interfaces.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        // User & Profile Repositories
        IAuthRepository AuthRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }      
        IGenericRepository<Admin> AdminRepository { get; }     
        //IGenericRepository<Student> StudentRepository { get; }
        IStudentRepository StudentRepository { get; }

        // Content Repositories
        IGenericRepository<Language> LanguageRepository { get; }
        IGenericRepository<Level> LevelRepository { get; }
        IGenericRepository<Stage> StageRepository { get; }
        IGenericRepository<MainActivity> MainActivityRepository { get; }
        IGenericRepository<ActivityType> ActivityTypeRepository { get; }
        IActivityRepository ActivityRepository { get; }
        IGenericRepository<Exercise> ExerciseRepository { get; }

        // Bridge Repository
        IGenericRepository<StudentProgress> StudentProgressRepository { get; } // <-- ADD THIS

        // The transactional save method
        Task<int> CompleteAsync();
    }
}

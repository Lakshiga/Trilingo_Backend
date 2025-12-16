using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Hashtable _repositories;

        // Repositories
        public IGenericRepository<Student> StudentRepository { get; private set; }
        public IGenericRepository<Activity> ActivityRepository { get; private set; }
        public IGenericRepository<Exercise> ExerciseRepository { get; private set; }
        public IGenericRepository<StudentProgress> StudentProgressRepository { get; private set; }
        public IGenericRepository<Level> LevelRepository { get; private set; }
        public IGenericRepository<Stage> StageRepository { get; private set; }
        public IGenericRepository<Role> RoleRepository { get; private set; }
        public IGenericRepository<User> UserRepository { get; private set; }
        public IGenericRepository<ActivityType> ActivityTypeRepository { get; private set; }
        public IGenericRepository<MainActivity> MainActivityRepository { get; private set; }
        
        // New repository for ExerciseAttempt
        public IGenericRepository<ExerciseAttempt> ExerciseAttemptRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new Hashtable();

            // Initialize repositories
            StudentRepository = new GenericRepository<Student>(context);
            ActivityRepository = new GenericRepository<Activity>(context);
            ExerciseRepository = new GenericRepository<Exercise>(context);
            StudentProgressRepository = new GenericRepository<StudentProgress>(context);
            LevelRepository = new GenericRepository<Level>(context);
            StageRepository = new GenericRepository<Stage>(context);
            RoleRepository = new GenericRepository<Role>(context);
            UserRepository = new GenericRepository<User>(context);
            ActivityTypeRepository = new GenericRepository<ActivityType>(context);
            MainActivityRepository = new GenericRepository<MainActivity>(context);
            
            // Initialize ExerciseAttempt repository
            ExerciseAttemptRepository = new GenericRepository<ExerciseAttempt>(context);
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T).Name;

            if (_repositories.ContainsKey(type))
            {
                return (IGenericRepository<T>)_repositories[type];
            }

            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);

            _repositories.Add(type, repositoryInstance);

            return (IGenericRepository<T>)repositoryInstance;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
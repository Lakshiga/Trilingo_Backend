using System;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Infrastructure.Data;
namespace TES_Learning_App.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IAuthRepository AuthRepository { get; private set; }
        public IStudentRepository StudentRepository { get; private set; } // Use the specific interface
        public IGenericRepository<User> UserRepository { get; private set; }
        public IGenericRepository<Role> RoleRepository { get; private set; }
        public IGenericRepository<Admin> AdminRepository { get; private set; }
        public IGenericRepository<Language> LanguageRepository { get; private set; }
        public IGenericRepository<Level> LevelRepository { get; private set; }
        public IGenericRepository<Stage> StageRepository { get; private set; }
        public IGenericRepository<MainActivity> MainActivityRepository { get; private set; }
        public IGenericRepository<ActivityType> ActivityTypeRepository { get; private set; }
        public IActivityRepository ActivityRepository { get; private set; }
        public IGenericRepository<Exercise> ExerciseRepository { get; private set; }
        public IGenericRepository<StudentProgress> StudentProgressRepository { get; private set; }
        public IGenericRepository<LevelPurchase> LevelPurchaseRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            AuthRepository = new AuthRepository(_context);
            StudentRepository = new StudentRepository(_context);
            UserRepository = new GenericRepository<User>(_context);
            RoleRepository = new GenericRepository<Role>(_context);
            AdminRepository = new GenericRepository<Admin>(_context);
            LanguageRepository = new GenericRepository<Language>(_context);
            LevelRepository = new GenericRepository<Level>(_context);
            StageRepository = new GenericRepository<Stage>(_context);
            MainActivityRepository = new GenericRepository<MainActivity>(_context);
            ActivityTypeRepository = new GenericRepository<ActivityType>(_context);
            ActivityRepository = new ActivityRepository(_context);
            ExerciseRepository = new GenericRepository<Exercise>(_context);
            StudentProgressRepository = new GenericRepository<StudentProgress>(_context);
            LevelPurchaseRepository = new GenericRepository<LevelPurchase>(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context?.Dispose();
    }
}
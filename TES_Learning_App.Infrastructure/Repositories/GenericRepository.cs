using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Infrastructure.Data;

namespace TES_Learning_App.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Implementation of your preferred GetById methods ---
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // --- Implementation of the new FindAsync method ---
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        // --- Implementation of FindByPropertyAsync method ---
        public async Task<IEnumerable<T>> FindByPropertyAsync(string propertyName, object value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(value);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await _context.Set<T>().Where(lambda).ToListAsync();
        }

        // --- Corrected Add, Update, and Delete methods ---
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);

            // Your original code was right. Returning the entity is good practice.
            return entity;
        }
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await Task.CompletedTask; // To match the Task return type
        }
        public Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            // We return a completed task to match the interface.
            return Task.CompletedTask;
        }
    }
}
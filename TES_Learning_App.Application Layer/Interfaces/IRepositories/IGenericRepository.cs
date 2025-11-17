using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.Interfaces.IRepositories
{
    // The '<T>' makes this a generic interface.
    // The 'where T : class' is a constraint that means T must be a class (like our entities).
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id); // Changed to int for content, but we can adapt this
        Task<T?> GetByIdAsync(Guid id); // Overload for Guid
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> FindByPropertyAsync(string propertyName, object value);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity); 
        Task DeleteAsync(T entity);
    }
}
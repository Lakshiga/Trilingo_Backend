using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TES_Learning_App.Infrastructure.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Student>> GetStudentsByParentIdAsync(Guid parentId)
        {
            // This query automatically filters out any "soft-deleted" children.
            return await _context.Students
                                 .Where(s => s.ParentId == parentId && !s.IsDeleted)
                                 .ToListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Interfaces.IRepositories
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<IEnumerable<Student>> GetStudentsByParentIdAsync(Guid parentId);
    }
}

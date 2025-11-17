using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Interfaces.Infrastructure
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}

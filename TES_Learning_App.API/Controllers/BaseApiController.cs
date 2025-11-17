using Microsoft.AspNetCore.Mvc;

namespace TES_Learning_App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        // This class is a perfect place to add any helper properties or methods
        // that you want ALL of your controllers to have access to in the future.
        // For example, getting the current user's ID from the token claims.
    }
}

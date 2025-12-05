using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.API.Controllers.Common;

namespace TES_Learning_App.API.Controllers.User
{
    /// <summary>
    /// Base controller for all User/Mobile app controllers
    /// Routes: api/user/* or api/* (for public endpoints)
    /// Authorization: AllowAnonymous by default (can be overridden per endpoint)
    /// </summary>
    [Route("api/[controller]")]
    [AllowAnonymous]
    public abstract class BaseUserController : BaseApiController
    {
    }
}


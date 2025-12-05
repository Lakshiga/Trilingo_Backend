using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.API.Controllers.Common;
using TES_Learning_App.Application_Layer.Common.Constants;

namespace TES_Learning_App.API.Controllers.Admin
{
    /// <summary>
    /// Base controller for all Admin panel controllers
    /// Routes: api/admin/*
    /// Authorization: Admin role required
    /// </summary>
    [Route("api/admin/[controller]")]
    [Authorize(Roles = RoleConstants.Admin)]
    public abstract class BaseAdminController : BaseApiController
    {
    }
}


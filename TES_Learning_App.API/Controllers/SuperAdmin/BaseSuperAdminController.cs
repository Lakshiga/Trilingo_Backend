using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.Common.Constants;
using TES_Learning_App.API.Controllers.Common;

namespace TES_Learning_App.API.Controllers.SuperAdmin
{
    /// <summary>
    /// Base controller for all SuperAdmin controllers
    /// Routes: api/superadmin/*
    /// Authorization: SuperAdmin role required
    /// </summary>
    [Route("api/superadmin/[controller]")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public abstract class BaseSuperAdminController : BaseApiController
    {
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TES_Learning_App.API.Services;
using TES_Learning_App.Application_Layer.DTOs.Common;

namespace TES_Learning_App.API.Filters
{
    /// <summary>
    /// Automatically validates ModelState for all actions - eliminates need to check in every controller method
    /// </summary>
    public class ModelStateValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorResponseService = context.HttpContext.RequestServices.GetRequiredService<IErrorResponseService>();
                var errorResponse = errorResponseService.CreateValidationErrorResponse<object>(context.ModelState);
                context.Result = new BadRequestObjectResult(errorResponse);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after execution
        }
    }
}


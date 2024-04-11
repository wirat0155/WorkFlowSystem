using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WorkFlowSystem.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Session.SetString("user.username", "010002");
            context.HttpContext.Session.SetString("user.role_no", "MNG");
            context.HttpContext.Session.SetString("user.department_no", "IT");
            base.OnActionExecuting(context);
        }
    }
}

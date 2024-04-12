using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WorkFlowSystem.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int user = 2;
            if (user == 1)
            {
                context.HttpContext.Session.SetString("user.username", "240002");
                context.HttpContext.Session.SetString("user.role_no", "EMP");
                context.HttpContext.Session.SetString("user.department_no", "IT");
            }
            else if (user == 2)
            {
                context.HttpContext.Session.SetString("user.username", "010001");
                context.HttpContext.Session.SetString("user.role_no", "ASM");
                context.HttpContext.Session.SetString("user.department_no", "IT");
            }
            else
            {

                context.HttpContext.Session.SetString("user.username", "010002");
                context.HttpContext.Session.SetString("user.role_no", "MNG");
                context.HttpContext.Session.SetString("user.department_no", "IT");
            }

           

            base.OnActionExecuting(context);
        }
    }
}

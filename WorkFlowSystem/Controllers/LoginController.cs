using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WorkFlowSystem.Models.ViewModels;
using WorkFlowSystem.Services;

namespace WorkFlowSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly DapperService _dapper;

        public LoginController(DapperService dapper)
        {
            _dapper = dapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [user] WHERE username = @username AND password = @password", 
                new { username = model.username, password = model.password });
            if(user != null)
            {
                setUser(user.username, user.role_no, user.department_no);
                return RedirectToAction("Index", "UserDoc");
            }
            else
            {
                TempData["username-error"] = "ชื่อผู้ใช้งาน หรือรหัสผิด";
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        private void setUser(string username, string role_no, string dept_no)
        {
            HttpContext.Session.SetString("user.username", username);
            HttpContext.Session.SetString("user.role_no", role_no);
            HttpContext.Session.SetString("user.department_no", dept_no);
        }
    }
}

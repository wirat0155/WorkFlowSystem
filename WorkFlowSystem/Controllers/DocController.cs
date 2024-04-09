using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WorkFlowSystem.Models.ViewModels;
using WorkFlowSystem.Repository;
using WorkFlowSystem.Services;

namespace WorkFlowSystem.Controllers
{
    public class DocController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WorkflowRepository _workflow;
        private readonly DapperService _dapper;

        public DocController(
            ILogger<HomeController> logger,
            WorkflowRepository workflow,
            DapperService dapper)
        {
            _logger = logger;
            _workflow = workflow;
            _dapper = dapper;
        }

        public async Task<IActionResult> RequestList()
        {
            HttpContext.Session.SetString("user.username", "010001");
            HttpContext.Session.SetString("user.role_no", "ASM");
            HttpContext.Session.SetString("user.department_no", "IT");
            string username = HttpContext.Session.GetString("user.username");
            string role_no = HttpContext.Session.GetString("user.role_no");
            string department_no = HttpContext.Session.GetString("user.department_no");
            var list = await _dapper.Query("SELECT * FROM [vw_user_doc] " +
                "WHERE department_no = @dep AND role_no = @rol AND disapprove_flag = 0" +
                "ORDER BY last_date ASC", new
            {
                dep = department_no,
                rol = role_no
            });
            return View(list);
        }

        public async Task<IActionResult> Detail(int id)
        {
            HttpContext.Session.SetString("user.username", "010001");
            HttpContext.Session.SetString("user.role_no", "ASM");
            HttpContext.Session.SetString("user.department_no", "IT");
            string username = HttpContext.Session.GetString("user.username");
            string role_no = HttpContext.Session.GetString("user.role_no");
            string department_no = HttpContext.Session.GetString("user.department_no");
            var wf_item = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [vw_user_doc] WHERE id = @id", new
            {
                id = id
            });
            return View(wf_item);
        }

        public async Task<IActionResult> Approve(int id)
        {
            HttpContext.Session.SetString("user.username", "010001");
            HttpContext.Session.SetString("user.role_no", "ASM");
            HttpContext.Session.SetString("user.department_no", "IT");
            string username = HttpContext.Session.GetString("user.username");
            string role_no = HttpContext.Session.GetString("user.role_no");
            string department_no = HttpContext.Session.GetString("user.department_no");


            var wf_item = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [vw_user_doc] WHERE id = @id", new
            {
                id = id
            });

            var wf_step_current = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [workflow_step] " +
                "WHERE workflow_no = @wf_no AND id = @id", new
                {
                    wf_no = wf_item.workflow_no,
                    id = wf_item.step_id
                });

            var wf_step_next = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [workflow_step] " +
                "WHERE workflow_no = @wf_no AND sequence = @sq AND revision_no = @rev", new
                {
                    wf_no = wf_item.workflow_no,
                    sq = wf_step_current.sequence + 1,
                    rev = wf_item.step_revision_no
                });

            await _dapper.Execute("UPDATE [workflow_item] SET step_id = @stp, last_date = @last WHERE id = @id", new
            {
                stp = wf_step_next.id,
                id = wf_item.id,
                last = DateTime.Now
            }) ;
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> Disapprove([FromForm] DisapproveVM model)
        {
            HttpContext.Session.SetString("user.username", "010001");
            HttpContext.Session.SetString("user.role_no", "ASM");
            HttpContext.Session.SetString("user.department_no", "IT");
            string username = HttpContext.Session.GetString("user.username");
            string role_no = HttpContext.Session.GetString("user.role_no");
            string department_no = HttpContext.Session.GetString("user.department_no");
            var wf_item = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [workflow_item] WHERE id = @id", new
            {
                id = model.id
            });

            await _dapper.Execute("INSERT INTO [wf_item_history] VALUES (@id, @stp, @us, @date, @act, @remarks)", new
            {
                id = model.id,
                stp = wf_item.step_id,
                us = username,
                date = DateTime.Now,
                act = "DIS",
                remarks = model.remarks
            });

            await _dapper.Execute("UPDATE [workflow_item] SET disapprove_flag = @flag, last_date = @date WHERE id = @id", new
            {
                flag = true,
                date = DateTime.Now,
                id = model.id
            });
            return RedirectToAction(nameof(RequestList));
        }
        
    }
}

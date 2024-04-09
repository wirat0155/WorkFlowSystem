using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using WorkFlowSystem.Models;
using WorkFlowSystem.Models.ViewModels;
using WorkFlowSystem.Repository;
using WorkFlowSystem.Services;

namespace WorkFlowSystem.Controllers
{
    public class UserDocController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WorkflowRepository _workflow;
        private readonly WorkflowItemService _wf_item;
        private readonly DapperService _dapper;

        public UserDocController(
            ILogger<HomeController> logger,
            WorkflowRepository workflow,
            WorkflowItemService wf_item,
            DapperService dapper)
        {
            _logger = logger;
            _workflow = workflow;
            _wf_item = wf_item;
            _dapper = dapper;
        }

        public async Task<IActionResult> Index()
        {
            HttpContext.Session.SetString("user.username", "010001");
            string username = HttpContext.Session.GetString("user.username");
            var list = await _dapper.Query("SELECT * FROM [vw_user_doc] WHERE username = @us AND sequence <> 1 ORDER BY last_date DESC", new
            {
                us = username
            });
            return View(list);
        }

        public async Task<IActionResult> DraftList()
        {
            HttpContext.Session.SetString("user.username", "010001");
            string username = HttpContext.Session.GetString("user.username");
            var list = await _dapper.Query("SELECT * FROM [vw_user_doc] WHERE username = @us AND sequence = 1 ORDER BY last_date DESC", new
            {
                us = username
            });
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var model = new WorkflowVM();
            model.Workflow = await _workflow.Get();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkflowVM model, string btn = "submit")
        {
            HttpContext.Session.SetString("user.username", "010001");
            string username = HttpContext.Session.GetString("user.username");
            if (!ModelState.IsValid)
            {
                model.Workflow = await _workflow.Get();
                return View(model);
            }


            var wf = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [workflow] WHERE workflow_no = @wf_no", new
            {
                wf_no = model.workflow_no
            });

            int sq = btn == "draft" ? 1 : 2;
            var wf_step = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [workflow_step] " +
                "WHERE workflow_no = @wf_no AND sequence = @sq AND revision_no = @rev", new
            {
                wf_no = model.workflow_no,
                sq,
                rev = wf?.revision_no
            });
            int? step_id = wf_step?.id;

            await _dapper.Execute("INSERT INTO [workflow_item] VALUES (@wf_no, @stp, @notes, @flag, @rev, @us, @cre, @last, @wf_item_no)", new
            {
                wf_no = model.workflow_no,
                stp = wf_step?.id,
                notes = model.notes,
                flag = false,
                rev = wf_step?.revision_no,
                us = username,
                cre = DateTime.Now,
                last = DateTime.Now,
                wf_item_no = await _wf_item.GenWfItemNo(model.workflow_no)
            }) ;

            if (btn == "draft")
                return RedirectToAction(nameof(DraftList));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = new WorkflowVM();
            model.Workflow = await _workflow.Get();
            var wf_item = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [workflow_item] WHERE id = @id", new
            {
                id = id
            });
            
            if (wf_item != null)
            {
                model.id = wf_item.id;
                model.notes = wf_item.notes;
                model.workflow_no = wf_item.workflow_no;
                model.complete_flag = wf_item.complete_flag;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WorkflowVM model, string btn = "submit")
        {
            HttpContext.Session.SetString("user.username", "010001");
            string username = HttpContext.Session.GetString("user.username");
            if (!ModelState.IsValid)
            {
                model.Workflow = await _workflow.Get();
                return View(model);
            }


            var wf = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [workflow] WHERE workflow_no = @wf_no", new
            {
                wf_no = model.workflow_no
            });

            int sq = btn == "draft" ? 1 : 2;
            var wf_step = await _dapper.QueryFirstOrDefaultAsync("SELECT * FROM [workflow_step] " +
                "WHERE workflow_no = @wf_no AND sequence = @sq AND revision_no = @rev", new
                {
                    wf_no = model.workflow_no,
                    sq,
                    rev = wf?.revision_no
                });
            int? step_id = wf_step?.id;

            await _dapper.Execute("UPDATE [workflow_item] SET workflow_no = @wf_no, step_id = @stp, notes = @notes, username = @us, last_date = @last WHERE id = @id", new
            {
                wf_no = model.workflow_no,
                stp = wf_step?.id,
                notes = model.notes,
                us = username,
                last = DateTime.Now,
                id = model.id
            });
            if (btn == "draft")
                return RedirectToAction(nameof(DraftList));
            return RedirectToAction(nameof(Index));
        }
    }
}

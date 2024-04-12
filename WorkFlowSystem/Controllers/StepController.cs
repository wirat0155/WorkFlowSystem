using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WorkFlowSystem.Models;
using WorkFlowSystem.Models.ViewModels;
using WorkFlowSystem.Repository;
using WorkFlowSystem.Services;

namespace WorkFlowSystem.Controllers
{
    public class StepController : BaseController
    {
        private readonly DapperService _dapper;
        private readonly DropdownService _dropdown;
        private readonly StepRepository _step;

        public StepController(
            DapperService dapper,
            DropdownService dropdown,
			StepRepository step)
        {
            _dapper = dapper;
            _dropdown = dropdown;
            _step = step;
        }
        public async Task<IActionResult> Index()
        {
            var step = await _dapper.Query("SELECT * FROM [vw_wf_step]");
            return View(step);
        }

        public async Task<IActionResult> StepList(string wf, int rev)
        {
            var step = await _dapper.Query("SELECT * FROM [vw_wf_step_list] " +
                "WHERE workflow_no = @wf AND revision_no = @rev " +
                "ORDER BY sequence ASC", new
                {
                    wf,
                    rev
                });

            int max_rev = await _step.GetMaxRevByWf(wf);

            SteplistVM model = new SteplistVM()
            {
                list = step,
                max_revision = max_rev
            };
            return View(model);
        }

        public async Task<IActionResult> EditStepList(string wf, int rev)
        {
            var step = await _dapper.Query("SELECT * FROM [workflow_step] " +
                "WHERE workflow_no = @wf AND revision_no = @rev", new
            {
                wf,
                rev
            });

            EditStepListVM model = new EditStepListVM();
            foreach(var item in step)
            {
				model.list.Add(new Vw_wf_step_list() {
					id = item.id,
					name = item.name,
					workflow_no = item.workflow_no,
					revision_no = item.revision_no,
					role_no = item.role_no,
					department_no = item.department_no,
					role_name = item.role_name,
					department_name = item.department_name,
					sequence = item.sequence,
					wf_name = item.wf_name
				});
			}
            
            model.department = await _dropdown.Department();
            model.role = await _dropdown.Role();
            return View(model);
        }

		[HttpPost]
		public async Task<IActionResult> SaveStep(List<SaveStepVM> list)
		{
			try
			{
                string username = HttpContext.Session.GetString("user.username");
                if (await CheckIsChange(list) == true)
                {
                    int max_rev = await _step.GetMaxRevByWf(list.First().workflow_no);
                    await _step.Insert(list, max_rev, username);
                    return Json(new { success = true, text = "เพิ่ม Revision ใหม่สำเร็จ" });
                }
                return Json(new { success = true, text = "ไม่มีการเปลี่ยนแปลง" });
            }
			catch (Exception ex)
			{
                return Json(new { success = false, text = "เกิดข้อผิดพลาด : " + ex.Message });
            }
		}

		private async Task<bool> CheckIsChange(List<SaveStepVM> list)
		{
			if (list.Count == 0)
				return false;

			var old_step = await _dapper.Query("SELECT * FROM [workflow_step] WHERE workflow_no = @wf AND revision_no = @rev", new
			{
				wf = list.First().workflow_no,
				rev = list.First().revision_no
			});

			// Check count
			// +2 for draft and approve
			if (old_step.Count() != (list.Count() + 2))
			{
				return true;
			}

			// Check content
			// not compare draft and approve
			int i = -1;
			foreach (var item in old_step)
			{
				i++;
				if (i == 0)
					continue;

				if (i + 1 == old_step.Count())
					break;

				if (item.name != list[i - 1].name ||
					item.role_no != list[i - 1].role_no ||
					item.department_no != list[i - 1].department_no)
					return true;

			}

			return false;
		}

	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WorkFlowSystem.Models.ViewModels;
using WorkFlowSystem.Repository;
using WorkFlowSystem.Services;

namespace WorkFlowSystem.Controllers
{
    public class StepController : BaseController
    {
        private readonly DapperService _dapper;
        private readonly DropdownService _dropdown;

        public StepController(
            DapperService dapper,
            DropdownService dropdown)
        {
            _dapper = dapper;
            _dropdown = dropdown;
        }
        public async Task<IActionResult> Index()
        {
            var step = await _dapper.Query("SELECT * FROM [vw_wf_step]");
            return View(step);
        }

        public async Task<IActionResult> StepList(string wf, int rev)
        {
            var step = await _dapper.Query("SELECT * FROM [workflow_step] " +
                "WHERE workflow_no = @wf AND revision_no = @rev " +
                "ORDER BY sequence ASC", new
                {
                    wf,
                    rev
                });
            return View(step);
        }

        public async Task<IActionResult> EditStepList(string wf, int rev)
        {
            var step = await _dapper.Query("SELECT * FROM [workflow_step] " +
                "WHERE workflow_no = @wf AND revision_no = @rev", new
            {
                wf,
                rev
            });

            StepListVM model = new StepListVM();
            model.list = step;
            model.department = await _dropdown.Department();
            model.role = await _dropdown.Role();
            return View(model);
        }

        
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkFlowSystem.Services;

namespace WorkFlowSystem.Repository
{
    public class WorkflowRepository
    {
        private readonly DapperService _dapper;

        public WorkflowRepository(DapperService dapper)
        {
            _dapper = dapper;
        }

        public async Task<List<SelectListItem>> Get()
        {
            var data = await _dapper.Query("SELECT * FROM [workflow]");
            var list = new List<SelectListItem>();
            foreach (var item in data)
            {
                list.Add(new SelectListItem()
                {
                   Value = item.workflow_no,
                   Text = item.workflow_no
                });
            }
            return list;
        }

    }
}

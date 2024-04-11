using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkFlowSystem.Services
{
    public class DropdownService
    {
        private readonly DapperService _dapper;

        public DropdownService(DapperService dapper)
        {
            _dapper = dapper;
        }

        public async Task<List<SelectListItem>> Department()
        {
            var departments = await _dapper.Query("SELECT * FROM [department]");

            return departments.Select(e => new SelectListItem
            {
                Value = e.department_no.ToString(),
                Text = e.department_name
            }).ToList();
        }

        public async Task<List<SelectListItem>> Role()
        {
            var departments = await _dapper.Query("SELECT * FROM [role]");

            return departments.Select(e => new SelectListItem
            {
                Value = e.role_no.ToString(),
                Text = e.name
            }).ToList();
        }

    }
}

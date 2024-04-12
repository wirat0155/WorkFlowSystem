using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkFlowSystem.Models.ViewModels;
using WorkFlowSystem.Services;

namespace WorkFlowSystem.Repository
{
    public class StepRepository
    {
        private readonly DapperService _dapper;

        public StepRepository(DapperService dapper)
        {
            _dapper = dapper;
        }
        public async Task<int> GetMaxRevByWf(string wf)
        {
            var revision = await _dapper.QueryFirstOrDefaultAsync("SELECT [max_revision_no] FROM [vw_wf_step] WHERE workflow_no = @wf", new { wf });
            return revision.max_revision_no ?? 0;
        }

        public async Task Insert(List<SaveStepVM> list, int max_rev, string us)
        {
            int i = 1;
            await _dapper.Execute("INSERT INTO [workflow_step] VALUES (@name, @seq, @wf, @rev, null, null)", new
            {
                name = "บันทึกแบบร่าง",
                seq = i++,
                wf = list.First().workflow_no,
                rev = max_rev + 1
            });

            foreach (var item in list)
            {
                await _dapper.Execute("INSERT INTO [workflow_step] VALUES (@name, @seq, @wf, @rev, @role, @dept)", new
                {
                    name = item.name,
                    seq = i++,
                    wf = item.workflow_no,
                    rev = max_rev + 1,
                    role = item.role_no,
                    dept = item.department_no
                });
            }
            await _dapper.Execute("INSERT INTO [workflow_step] VALUES (@name, @seq, @wf, @rev, null, null)", new
            {
                name = "อนุมัติ",
                seq = i++,
                wf = list.First().workflow_no,
                rev = max_rev + 1
            });

            await _dapper.Execute("INSERT INTO [wf_revision_history] VALUES(@wf, @rev, @date, @us)", new
            {
                wf = list.First().workflow_no,
                rev = max_rev + 1,
                date = DateTime.Now,
                us = us
            });
        }
    }
}

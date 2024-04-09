using System;
using System.Globalization;
using System.Threading.Tasks;
using WorkFlowSystem.Models.ViewModels;

namespace WorkFlowSystem.Services
{
    public class WorkflowItemService
    {
        private readonly DapperService _dapper;

        public WorkflowItemService(DapperService dapper)
        {
            _dapper = dapper;
        }
        public async Task<string> GenWfItemNo(string wf_no)
        {
            var data = await _dapper.QueryFirstOrDefaultAsync("SELECT COUNT(id) AS Count FROM [workflow_item] " +
                "WHERE CONVERT(DATE, cre_date) = @date", new
                {
                    date = DateTime.Now.Date
                });

            string paddedCount = (data.Count + 1).ToString("000");
            var englishCulture = new CultureInfo("en-US");
            string currentDate = DateTime.Now.ToString("yyyyMMdd", englishCulture);

            return wf_no + currentDate + paddedCount;
        }
    }
}

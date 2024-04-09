using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WorkFlowSystem.Models.ViewModels
{
    public class WorkflowVM
    {
        public int id { get; set; }
        public string workflow_no { get; set; }
        public int step_id { get; set; }
        public string notes { get; set; }
        public bool complete_flag { get; set; }

        [ValidateNever]
        public List<SelectListItem> Workflow { get; set; }
    }
}

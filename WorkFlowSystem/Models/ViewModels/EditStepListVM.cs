using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace WorkFlowSystem.Models.ViewModels
{
    public class EditStepListVM
    {
        public List<Vw_wf_step_list> list { get; set; } = new List<Vw_wf_step_list>();
        public List<SelectListItem> department { get; set; }
        public List<SelectListItem> role { get; set; }
    }
}

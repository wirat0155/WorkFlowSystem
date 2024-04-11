using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace WorkFlowSystem.Models.ViewModels
{
    public class StepListVM
    {
        public IEnumerable<dynamic> list;
        public List<SelectListItem> department { get; set; }
        public List<SelectListItem> role { get; set; }
    }
}

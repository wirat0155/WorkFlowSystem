using System.Collections.Generic;

namespace WorkFlowSystem.Models.ViewModels
{
    public class SteplistVM
    {
        public IEnumerable<dynamic> list { get; set; }
        public int? max_revision  { get; set; }
    }
}

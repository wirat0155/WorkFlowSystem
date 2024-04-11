namespace WorkFlowSystem.Models
{
	public class Vw_wf_step_list
	{
		public int id { get; set; }
		public string name { get; set; }
		public string workflow_no { get; set; }
		public int revision_no { get; set; }
		public string? role_no { get; set; }
		public string? department_no { get; set; }
		public string? role_name { get; set; }
		public string? department_name { get; set; }
		public int sequence { get; set; }
		public string wf_name { get; set; }
	}
}

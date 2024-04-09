using System.ComponentModel.DataAnnotations;

namespace WorkFlowSystem.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}

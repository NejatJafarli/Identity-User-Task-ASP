using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace App.MvcWebUI.Models
{
    public class CreateRoleViewModel
    {
        public string RoleName { get; set; }
        [Display(Name = "User Claims")]
        public List<SelectListItem> Claims { get; set; }
    }
}

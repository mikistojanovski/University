using rsweb10.Models;
using System.ComponentModel.DataAnnotations;

namespace rsweb10.ViewModels
{
    public class EditAsStudent
    {
        public Enrollment Enrollment { get; set; }

        [Display(Name = "Seminal File")]
        public IFormFile? SeminalUrlFile { get; set; }

        public string? SeminalUrlName { get; set; }
    }
}

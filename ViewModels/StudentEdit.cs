using rsweb10.Models;
using System.ComponentModel.DataAnnotations;

namespace rsweb10.ViewModels
{
    public class StudentEdit
    {
        public Enrollment enrollment { get; set; }

        [Display(Name = "Seminal File")]
        public IFormFile? seminalUrlFile { get; set; }

        public string? seminalUrlName { get; set; }
    }
}

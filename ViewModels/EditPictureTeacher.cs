using rsweb10.Models;
using System.ComponentModel.DataAnnotations;

namespace rsweb10.ViewModels
{
    public class EditPictureTeacher
    {
        public Teacher? teacher { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? profilePictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? profilePictureName { get; set; }
    }
}

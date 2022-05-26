using rsweb10.Models;
using System.ComponentModel.DataAnnotations;

namespace rsweb10.ViewModels
{
    public class EditPictureStudent
    {
        public Student? student { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? profilePictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? profilePictureName { get; set; }
    }
}

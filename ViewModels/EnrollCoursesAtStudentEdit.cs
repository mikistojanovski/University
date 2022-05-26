using Microsoft.AspNetCore.Mvc.Rendering;
using rsweb10.Models;
using System.ComponentModel.DataAnnotations;

namespace rsweb10.ViewModels
{
    public class EnrollCoursesAtStudentEdit
    {
        public Student student { get; set; }

        public IEnumerable<int>? selectedCourses { get; set; }

        public IEnumerable<SelectListItem>? coursesEnrolledList { get; set; }

        [Display(Name = "Year")]
        public int? year { get; set; }

        [Display(Name = "Semester")]
        public string? semester { get; set; }

        public string? profilePictureName { get; set; }
    }
}

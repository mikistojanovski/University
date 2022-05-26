using Microsoft.AspNetCore.Mvc.Rendering;
using rsweb10.Models;

namespace rsweb10.ViewModels
{
    public class EnrollStudentsAtCourseEdit
    {
        public Course course { get; set; }

        public IEnumerable<int>? selectedStudents { get; set; }
        
        public IEnumerable<SelectListItem>? studentsEnrolledList { get; set; }

        public int? year { get; set; }
    }
}

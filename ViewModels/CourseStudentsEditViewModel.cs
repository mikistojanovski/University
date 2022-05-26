using Microsoft.AspNetCore.Mvc.Rendering;
using rsweb10.Models;

namespace rsweb10.ViewModels
{
    public class CourseStudentsEditViewModel
    {
        public Course Course { get; set; }
        public IEnumerable<int> SelectedStudents { get; set; }
        public IEnumerable<SelectListItem> StudentList { get; set; }

    }
}

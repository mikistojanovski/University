using Microsoft.AspNetCore.Mvc.Rendering;
using rsweb10.Models;

namespace rsweb10.ViewModels
{
    public class CourseSearchViewModel
    {
        public IList<Course> Course { get; set; }
        public SelectList Programme { get; set; }
        public SelectList Semesters { get; set; }
        public string CourseProgramme { get; set; }
        public int CourseSemester { get; set; }
        public string SearchString { get; set; }
    }
}

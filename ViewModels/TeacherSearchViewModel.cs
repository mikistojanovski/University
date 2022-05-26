using Microsoft.AspNetCore.Mvc.Rendering;
using rsweb10.Models;

namespace rsweb10.ViewModels
{
    public class TeacherSearchViewModel
    {
        public IList<Teacher> Teacher { get; set; }
        public SelectList Degree { get; set; }
        public SelectList AcademicRank { get; set; }
        public string DegreeProgramme { get; set; }
        public string ARSemester { get; set; }
        public string SearchString { get; set; }
    }
}

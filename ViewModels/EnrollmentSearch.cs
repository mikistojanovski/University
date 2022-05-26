using Microsoft.AspNetCore.Mvc.Rendering;
using rsweb10.Models;

namespace rsweb10.ViewModels
{
    public class EnrollmentSearch
    {
        public IList<Enrollment> enrollments { get; set; }

        public SelectList yearsList { get; set; }
        public int year { get; set; }
    }
}

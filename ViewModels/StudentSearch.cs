using rsweb10.Models;

namespace rsweb10.ViewModels
{
    public class StudentSearch
    {
        public IList<Student> Students { get; set; }

        public string Fullname { get; set; }

        public string StudentID { get; set; }
    }
}

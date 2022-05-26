using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rsweb10.Models
{
    public class Course
    {
        [Required]
        public int CourseID { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public int Credits { get; set; }
        public int Semester { get; set; }
        [StringLength(100)]
        public string? Programme { get; set; }
        [Display(Name = "Ecudation Level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }



        [ForeignKey("FirstTeacherId")]
        [Display(Name = "First Teacher")]
        public int? FirstTeacherId { get; set; }
        public Teacher? FirstTeacher { get; set; }

        
        [ForeignKey("SecondTeacherId")]
        [Display(Name ="Second Teacher")]
        public int? SecondTeacherId { get; set; }
        public Teacher? SecondTeacher { get; set; }


        public ICollection<Enrollment>? Students { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace rsweb10.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        [Display(Name = "Course")]
        public int CourseID { get; set; }
        [Display(Name = "Course")]
        public Course? Course { get; set; }
        [Display(Name = "Student")]
        public int StudentID { get; set; }
        [Display(Name = "Student")]
        public Student? Student { get; set; }
        [Display(Name = "Semester")]
        [StringLength(10)]
        public string? Semester { get; set; }
        [Display(Name = "Year")]
        public int? Year { get; set; }
        [Display(Name = "Grade")]
        public Nullable<int> Grade { get; set; }
        [StringLength(255)]
        [Display(Name = "Seminal Url")]
        public string? SeminalUrl { get; set; }
        [Display(Name = "Project Url")]
        [StringLength(255)]
        public string? ProjectUrl { get; set; }
        [Display(Name = "Exam Points")]
        public int? ExamPoints { get; set; }
        [Display(Name = "Seminal Points")]
        public Nullable<int> SeminalPoints { get; set; }
        [Display(Name = "Project Points")]

        public Nullable<int> ProjectPoints { get; set; }
        [Display(Name = "Additional Points")]

        public Nullable<int> AdditionalPoints { get; set; }
        [Display(Name = "Finish Date")]

        public Nullable<DateTime> FinishDate { get; set; }
        public int totalPoints
        {
            get
            {
                if (ExamPoints == null)
                    ExamPoints = 0;
                if (SeminalPoints == null)
                    SeminalPoints = 0;
                if (ProjectPoints == null)
                    ProjectPoints = 0;
                if (AdditionalPoints == null)
                    AdditionalPoints = 0;
                int points = (int)(ExamPoints + SeminalPoints + ProjectPoints + AdditionalPoints);
                return (int)points;
            }
        }
    }
}

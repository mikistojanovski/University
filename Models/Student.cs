using rsweb10.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace rsweb10.Models
{
    public class Student
    {
        public string? userIdentityId { get; set; }

        public rsweb10User? userIdentity { get; set; }

        [Required]
        public int ID { get; set; }
        [Required]
        [StringLength(10)]
        public string StudentId { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public Nullable<DateTime> EnrollmentDate { get; set; }
        [Display(Name = "Acquired Credits")]
        public Nullable<int> AcquiredCredits { get; set; }
        [Display(Name = "Current Semestar")]
        public Nullable<int> CurrentSemestar { get; set; }
        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }


        public string? ProfilePicture { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public ICollection<Enrollment>? Courses { get; set; }


    }
}

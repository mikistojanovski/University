using rsweb10.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace rsweb10.Models
{
    public class Teacher
    {
        public string? userIdentityId { get; set; }

        public rsweb10User? userIdentity { get; set; }
        [Required]
        public int TeacherId { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [StringLength(50)]
        public string? Degree { get; set; }
        [Display(Name = "Academic Rank")]
        [StringLength(25)]
        public string? AcademicRank { get; set; }
        [Display(Name = "Office Number")]
        [StringLength(10)]
        public string? OfficeNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; }
        [Display(Name = "Full Name")]
        public string? ProfilePicture { get; set; }
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public ICollection<Course>? Course1 { get; set; }
        public ICollection<Course>? Course2 { get; set; }
    }
}

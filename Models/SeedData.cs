using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using rsweb10.Areas.Identity.Data;
using rsweb10.Models;

namespace rsweb10.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<rsweb10User>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            rsweb10User user = await UserManager.FindByEmailAsync("admin@rsweb10.com");
            if (user == null)
            {
                var User = new rsweb10User();
                User.Email = "admin@rsweb10.com";
                User.UserName = "admin@rsweb10.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }

            //Add Teacher Role
            roleCheck = await RoleManager.RoleExistsAsync("Teacher");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Teacher")); }
            user = await UserManager.FindByEmailAsync("severus@rsweb10.com");
            if (user == null)
            {
                var User = new rsweb10User();
                User.Email = "severus@rsweb10.com";
                User.UserName = "severus@rsweb10.com";
                string userPWD = "Teacher123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Teacher
                if (chkUser.Succeeded)
                {
                    var result1 = await UserManager.AddToRoleAsync(User, "Teacher");
                }
            }
            user = await UserManager.FindByEmailAsync("");
            if (user == null)
            {
                var User = new rsweb10User();
                User.Email = "";
                User.UserName = "";
                string userPWD = "Teacher123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Teacher
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Teacher"); }
            }

            //Add Student Role
            roleCheck = await RoleManager.RoleExistsAsync("Student");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Student")); }
            user = await UserManager.FindByEmailAsync("harry@rsweb10.com");
            if (user == null)
            {
                var User = new rsweb10User();
                User.Email = "harry@rsweb10.com";
                User.UserName = "harry@rsweb10.com";
                string userPWD = "Student123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Student
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Student"); }
            }
            user = await UserManager.FindByEmailAsync("hermione@rsweb10.com");
            if (user == null)
            {
                var User = new rsweb10User();
                User.Email = "hermione@rsweb10.com";
                User.UserName = "hermione@rsweb10.com";
                string userPWD = "Student123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Student
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Student"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new rsweb10Context(
 serviceProvider.GetRequiredService<
 DbContextOptions<rsweb10Context>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                if (context.Course.Any() || context.Teacher.Any() || context.Student.Any())
                {
                    return; // DB has been seeded
                }


                var students = new Student[] {
                    new Student
                {
                    StudentId = "1000",
                    FirstName = "Harry",
                    LastName = "Potter",
                    EnrollmentDate = DateTime.Parse("2018-09-01"),
                    AcquiredCredits = 100,
                    CurrentSemestar = 4,
                    EducationLevel = "High School Diploma",
                    ProfilePicture = "https://upload.wikimedia.org/wikipedia/sco/4/44/HarryPotter5poster.jpg",
                        userIdentityId = context.Users.Single(x => x.Email == "harry@rsweb10.com").Id

                   },
                     new Student
                     {
                         StudentId = "1001",
                         FirstName = "Hermione",
                         LastName = "Granger",
                         EnrollmentDate = DateTime.Parse("2018-09-01"),
                         AcquiredCredits = 100,
                         CurrentSemestar = 4,
                         EducationLevel = "High School Diploma",
                         ProfilePicture = "https://myhero.com/images/guest/g282317/hero105677/image2.jpg",
                         userIdentityId = context.Users.Single(x => x.Email == "hermione@rsweb10.com").Id
                     },
            new Student
            {
                StudentId = "1002",
                FirstName = "Ronald",
                LastName = "Weasley",
                EnrollmentDate = DateTime.Parse("2018-09-01"),
                AcquiredCredits = 100,
                CurrentSemestar = 4,
                EducationLevel = "High School Diploma",
                ProfilePicture = "https://vignette.wikia.nocookie.net/harrypotterfanon/images/5/56/Ron_Weasley_%28Scopatore%29.jpg/revision/latest?cb=20160120050600"
            },
            new Student
            {
                StudentId = "1003",
                FirstName = "Ginny",
                LastName = "Weasley",
                EnrollmentDate = DateTime.Parse("2018-09-01"),
                AcquiredCredits = 100,
                CurrentSemestar = 4,
                EducationLevel = "High School Diploma",
                ProfilePicture = "https://vignette.wikia.nocookie.net/harrypotter/images/8/8b/Ginny_Weasley_hbp_promo.jpg/revision/latest?cb=20180322181904"
            },
            new Student
            {
                StudentId = "1004",
                FirstName = "Neville",
                LastName = "Longbottom",
                EnrollmentDate = DateTime.Parse("2018-09-01"),
                AcquiredCredits = 100,
                CurrentSemestar = 4,
                EducationLevel = "High School Diploma",
                ProfilePicture = "https://vignette.wikia.nocookie.net/harrypotter/images/1/10/Neville-promo-pics-neville-longbottom-28261912-390-520.jpg/revision/latest?cb=20140610141612"
            },
            new Student
            {
                StudentId = "1005",
                FirstName = "Luna",
                LastName = "Lovegood",
                EnrollmentDate = DateTime.Parse("2018-09-01"),
                AcquiredCredits = 100,
                CurrentSemestar = 4,
                EducationLevel = "High School Diploma",
                ProfilePicture = "https://images.ctfassets.net/usf1vwtuqyxm/t6GVMDanqSKGOKaCWi8oi/74b6816d9f913623419b98048ec87d25/LunaLovegood_WB_F5_LunaLovegoodPromoCloseUp_Promo_080615_Port.jpg?fm=jpg"
            },
            new Student
            {
                StudentId = "1006",
                FirstName = "Draco",
                LastName = "Malfoy",
                EnrollmentDate = DateTime.Parse("2018-09-01"),
                AcquiredCredits = 100,
                CurrentSemestar = 4,
                EducationLevel = "High School Diploma",
                ProfilePicture = "https://vignette.wikia.nocookie.net/hogwarts-life/images/8/82/Draco_Malfoy.jpg/revision/latest?cb=20170112183355"
            },
            new Student
            {
                StudentId = "1007",
                FirstName = "Cedric",
                LastName = "Diggory",
                EnrollmentDate = DateTime.Parse("2018-09-01"),
                AcquiredCredits = 100,
                CurrentSemestar = 4,
                EducationLevel = "High School Diploma",
                ProfilePicture = "https://images-na.ssl-images-amazon.com/images/I/51-mjy5XsML._AC_.jpg"
            }
            };

                foreach (Student s in students)
                {
                    context.Student.Add(s);
                }

                context.SaveChanges();
                var teachers = new Teacher[]
           {
            new Teacher{ FirstName="Severus", LastName="Snape", Degree="Doctorate", AcademicRank="Full professor",
             OfficeNumber="304",HireDate=DateTime.Parse("2003-07-01"), ProfilePicture="https://lh3.googleusercontent.com/proxy/Oz8V-0G0RvI5Emt150FC2_mp2GmYz0CVuacTqiqTz52dE7RugmdMlztlIO42DGhXhCdarepKzOWsS_Lf4rdu26EEf0qZyXedGqPLtDe366i6nIInK65CdU8Tmg"
             ,userIdentityId = context.Users.Single(x => x.Email == "severus@rsweb10.com").Id
                  },
            new Teacher{ FirstName="Minerva", LastName="McGonagall", Degree="Doctorate", AcademicRank="Full professor",
             OfficeNumber="201",HireDate=DateTime.Parse("2004-08-02"), ProfilePicture="https://i.pinimg.com/originals/8b/71/30/8b71304ce2660f2943b2639ae2a6ebba.jpg"},
            new Teacher{ FirstName="Albus", LastName="Dumbledore", Degree="Doctorate", AcademicRank="Full professor",
             OfficeNumber="804",HireDate=DateTime.Parse("2002-06-05"), ProfilePicture="https://images.ctfassets.net/usf1vwtuqyxm/1dmmUJzpRcWaUmMOCu8QwO/7e013145694566076d47fd004fd604c2/AlbusDumbledore_WB_F6_DumbledoreSittingInChair_Promo_080615_Port.jpg?fm=jpg"},
            new Teacher{ FirstName="Filius", LastName="Flitwick", Degree="Doctorate", AcademicRank="Full professor",
             OfficeNumber="312",HireDate=DateTime.Parse("2003-04-23"), ProfilePicture="https://images.ctfassets.net/usf1vwtuqyxm/7zwF0icg0kQW2ejGM7mqw2/29f2491ad063a317fb55a6f6c69aecce/flitwich-quiz-image.jpg?fm=jpg"},
            new Teacher{ FirstName="Pomona", LastName="Sprout", Degree="Doctorate", AcademicRank="Full professor",
             OfficeNumber="901",HireDate=DateTime.Parse("2005-09-23"), ProfilePicture="https://vignette.wikia.nocookie.net/hogwarts-life/images/7/7d/HP72-FP-00573.jpg/revision/latest/top-crop/width/360/height/450?cb=20170122163222"},
            new Teacher{ FirstName="Horace", LastName="Slughorn", Degree="Doctorate", AcademicRank="Full professor",
            OfficeNumber="134",HireDate=DateTime.Parse("2004-02-12"), ProfilePicture="https://pbs.twimg.com/media/EAezZ-gXUAU3aD-.jpg"},
            new Teacher{ FirstName="Rubeus", LastName="Hagrid", Degree="Doctorate", AcademicRank="Full professor",
             OfficeNumber="784",HireDate=DateTime.Parse("2002-08-16"), ProfilePicture="https://images-na.ssl-images-amazon.com/images/I/51K6H%2BEhr1L._AC_.jpg"}
           };
                foreach (Teacher t in teachers)
                {
                    context.Teacher.Add(t);
                    context.SaveChanges();
                }
                context.SaveChanges();

                var courses = new Course[]
           {
            new Course{Title="Potions",Credits=3,Semester=4,Programme="New programme", EducationLevel="High School Diploma",
            FirstTeacherId=teachers.Single(s=>s.LastName=="Snape").TeacherId, SecondTeacherId=teachers.Single(s=>s.LastName=="McGonagall").TeacherId },
            new Course{Title="History of Magic",Credits=3,Semester=4,Programme="New programme", EducationLevel="High School Diploma",
            FirstTeacherId=teachers.Single(s=>s.LastName=="Snape").TeacherId, SecondTeacherId=teachers.Single(s=>s.LastName=="McGonagall").TeacherId},
            new Course{Title="Defence Againts the Dark Arts",Credits=3,Semester=4,Programme="New programme", EducationLevel="High School Diploma",
            FirstTeacherId=teachers.Single(s=>s.LastName=="Dumbledore").TeacherId, SecondTeacherId=teachers.Single(s=>s.LastName=="Flitwick").TeacherId},
            new Course{Title="Care of Magical Creatures",Credits=3,Semester=4,Programme="New programme", EducationLevel="High School Diploma",
            FirstTeacherId=teachers.Single(s=>s.LastName=="Dumbledore").TeacherId, SecondTeacherId=teachers.Single(s=>s.LastName=="Flitwick").TeacherId},
            new Course{Title="Astronomy",Credits=4,Semester=4,Programme="New programme", EducationLevel="High School Diploma",
            FirstTeacherId=teachers.Single(s=>s.LastName=="Sprout").TeacherId, SecondTeacherId=teachers.Single(s=>s.LastName=="Slughorn").TeacherId},
            new Course{Title="Flying",Credits=3,Semester=4,Programme="New programme", EducationLevel="High School Diploma",
            FirstTeacherId=teachers.Single(s=>s.LastName=="Sprout").TeacherId, SecondTeacherId=teachers.Single(s=>s.LastName=="Hagrid").TeacherId},
            new Course{Title="Study of Ancient Runes",Credits=4,Semester=4,Programme="New programme", EducationLevel="High School Diploma",
            FirstTeacherId=teachers.Single(s=>s.LastName=="Slughorn").TeacherId, SecondTeacherId=teachers.Single(s=>s.LastName=="Hagrid").TeacherId}
           };
                foreach (Course c in courses)
                {
                    context.Course.Add(c);
                    context.SaveChanges();
                }
                context.SaveChanges();

                var enrollments = new Enrollment[]
{
            new Enrollment{ StudentID=students.SingleOrDefault(s => s.LastName == "Potter").ID,CourseID= courses.SingleOrDefault(c => c.Title == "Potions").CourseID, Semester="4", Year=2,
            Grade=10, SeminalUrl="abc", ProjectUrl="ab1",ExamPoints=100, SeminalPoints=100, ProjectPoints=100,AdditionalPoints=5, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Potter").ID,CourseID=courses.Single(c => c.Title == "History of Magic").CourseID,Semester="4", Year=2,
             Grade=9, SeminalUrl="abd", ProjectUrl="ab2", ExamPoints=80, SeminalPoints=90, ProjectPoints=100,AdditionalPoints=0, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Granger").ID,CourseID=courses.Single(c => c.Title == "History of Magic").CourseID,Semester="4", Year=2,
             Grade=8, SeminalUrl="abe", ProjectUrl="ab3", ExamPoints=50, SeminalPoints=80, ProjectPoints=100,AdditionalPoints=3, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Granger").ID,CourseID=courses.Single(c => c.Title == "Defence Againts the Dark Arts").CourseID,Semester="4", Year=2,
             Grade=7, SeminalUrl="abf", ProjectUrl="ab4", ExamPoints=70, SeminalPoints=50, ProjectPoints=50,AdditionalPoints=2, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.FirstName == "Ronald").ID,CourseID=courses.Single(c => c.Title == "Defence Againts the Dark Arts").CourseID,Semester="4", Year=2,
             Grade=6, SeminalUrl="abg", ProjectUrl="ab5", ExamPoints=50, SeminalPoints=50, ProjectPoints=50,AdditionalPoints=5, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.FirstName == "Ronald").ID,CourseID=courses.Single(c => c.Title == "Care of Magical Creatures").CourseID,Semester="4", Year=2,
             Grade=6, SeminalUrl="abh", ProjectUrl="ab6", ExamPoints=40, SeminalPoints=50, ProjectPoints=60,AdditionalPoints=3, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.FirstName == "Giny").ID,CourseID=courses.Single(c => c.Title == "Care of Magical Creatures").CourseID,Semester="4", Year=2,
             Grade=9, SeminalUrl="abi", ProjectUrl="ab7", ExamPoints=70, SeminalPoints=100, ProjectPoints=60,AdditionalPoints=6, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.FirstName == "Giny").ID,CourseID=courses.Single(c => c.Title == "Astronomy").CourseID,Semester="4", Year=2,
             Grade=9, SeminalUrl="abj", ProjectUrl="ab8", ExamPoints=80, SeminalPoints=90, ProjectPoints=100,AdditionalPoints=12, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Longbottom").ID,CourseID=courses.Single(c => c.Title == "Astronomy").CourseID,Semester="4", Year=2,
             Grade=6, SeminalUrl="abk", ProjectUrl="ab9", ExamPoints=50, SeminalPoints=30, ProjectPoints=40,AdditionalPoints=8, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Longbottom").ID,CourseID=courses.Single(c => c.Title == "Flying").CourseID,Semester="4", Year=2,
             Grade=8, SeminalUrl="abl", ProjectUrl="ab10", ExamPoints=100, SeminalPoints=50, ProjectPoints=100,AdditionalPoints=0, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Lovegood").ID,CourseID=courses.Single(c => c.Title == "Flying").CourseID,Semester="4", Year=2,
             Grade=7, SeminalUrl="abm", ProjectUrl="ab11", ExamPoints=70, SeminalPoints=70, ProjectPoints=70,AdditionalPoints=10, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Lovegood").ID,CourseID=courses.Single(c => c.Title == "Study of Ancient Runes").CourseID,Semester="4", Year=2,
             Grade=10, SeminalUrl="abn", ProjectUrl="ab12", ExamPoints=100, SeminalPoints=100, ProjectPoints=100,AdditionalPoints=3, FinishDate=DateTime.Parse("2020-10-01")},
             new Enrollment{ StudentID=students.Single(s => s.LastName == "Malfoy").ID,CourseID=courses.Single(c => c.Title == "Study of Ancient Runes").CourseID,Semester="4", Year=2,
              Grade=6, SeminalUrl="abo", ProjectUrl="ab13", ExamPoints=50, SeminalPoints=30, ProjectPoints=40,AdditionalPoints=8, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Malfoy").ID,CourseID=courses.Single(c => c.Title == "Defence Againts the Dark Arts").CourseID,Semester="4", Year=2,
             Grade=8, SeminalUrl="abp", ProjectUrl="ab14", ExamPoints=100, SeminalPoints=50, ProjectPoints=100,AdditionalPoints=0, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Diggory").ID,CourseID=courses.Single(c => c.Title == "Flying").CourseID,Semester="4", Year=2,
             Grade=7, SeminalUrl="abq", ProjectUrl="ab15", ExamPoints=70, SeminalPoints=70, ProjectPoints=70,AdditionalPoints=10, FinishDate=DateTime.Parse("2020-10-01")},
            new Enrollment{ StudentID=students.Single(s => s.LastName == "Diggory").ID,CourseID=courses.Single(c => c.Title == "Potions").CourseID,Semester="4", Year=2,
             Grade=10, SeminalUrl="abr", ProjectUrl="ab16", ExamPoints=100, SeminalPoints=100, ProjectPoints=100,AdditionalPoints=3, FinishDate=DateTime.Parse("2020-10-01")}
};
                foreach (Enrollment e in enrollments)
                {
                    context.Enrollment.Add(e);
                }
                context.SaveChanges();

            }


        }
        }
    }

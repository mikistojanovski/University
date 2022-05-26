using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rsweb10.Areas.Identity.Data;
using rsweb10.Models;
using rsweb10.ViewModels;

namespace rsweb10.Controllers
{
    public class StudentsController : Controller
    {
        private readonly rsweb10Context _context;
        private UserManager<rsweb10User> _userManager;
        public StudentsController(rsweb10Context context, UserManager<rsweb10User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Students
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string Fullname, string StudentID)
        {
            IQueryable<Student> studentsQuery = _context.Student.AsQueryable();
            if (!string.IsNullOrEmpty(Fullname))
            {
                if (Fullname.Contains(" "))
                {
                    string[] names = Fullname.Split(" ");
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(names[0]) || x.LastName.Contains(names[1]) ||
                    x.FirstName.Contains(names[1]) || x.LastName.Contains(names[0]));
                }
                else
                {
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(Fullname) || x.LastName.Contains(Fullname));
                }
            }
            if (!string.IsNullOrEmpty(StudentID))
            {
                studentsQuery = studentsQuery.Where(x => x.StudentId.Contains(StudentID));
            }
            var StudentSearchVM = new StudentSearch
            {
                Students = await studentsQuery.ToListAsync()
            };

            return View(StudentSearchVM);
        }

        // GET: Students/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            EditPictureStudent viewmodel = new EditPictureStudent
            {
                student = student,
                profilePictureName = student.ProfilePicture
            };
            return View(viewmodel);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Courses"] = new SelectList(_context.Set<Course>(), "CourseId", "Title");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel,ProfilePicture,Courses")] Student student)
        {
            if (ModelState.IsValid)
            {
                var User = new rsweb10User();
                User.Email = student.FirstName.ToLower() + "." + student.StudentId.ToLower() + "@rsweb10.com";
                User.UserName = student.FirstName.ToLower() + "." + student.LastName.ToLower();
                string userPWD = "Student123";
                IdentityResult chkUser = await _userManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await _userManager.AddToRoleAsync(User, "Student"); }
                student.userIdentityId = User.Id;
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Student.Where(x => x.ID == id).Include(x => x.Courses).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Course.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            EnrollCoursesAtStudentEdit VM = new EnrollCoursesAtStudentEdit
            {
                student = student,
                coursesEnrolledList = new MultiSelectList(courses, "courseId", "title"),
                selectedCourses = student.Courses.Select(x => x.CourseID)
            };

            ViewData["Courses"] = new SelectList(_context.Set<Course>(), "courseId", "title");
            return View(VM);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, EnrollCoursesAtStudentEdit viewmodel)
        {
            if (id != viewmodel.student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.student);
                    await _context.SaveChangesAsync();

                    var student = _context.Student.Where(x => x.ID == id).First();

                    IEnumerable<int> selectedCourses = viewmodel.selectedCourses;
                    if (selectedCourses != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !selectedCourses.Contains(s.CourseID) && s.StudentID == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);

                        IEnumerable<int> existEnrollments = _context.Enrollment.Where(s => selectedCourses.Contains(s.CourseID) && s.StudentID == id).Select(s => s.CourseID);
                        IEnumerable<int> newEnrollments = selectedCourses.Where(s => !existEnrollments.Contains(s));

                        foreach (int courseId in newEnrollments)
                            _context.Enrollment.Add(new Enrollment { StudentID = id, CourseID = courseId, Semester = viewmodel.semester, Year = viewmodel.year });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => s.StudentID == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewmodel);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Student == null)
            {
                return Problem("Entity set 'rsweb10Context.Student'  is null.");
            }
            var student = await _context.Student.FindAsync(id);
            if (student != null)
            {
                _context.Student.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
          return (_context.Student?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        // GET: Students/StudentsEnrolled/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StudentsEnrolled(int? id,string Fullname, string StudentID)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.CourseID == id);
            ViewBag.Message = course.Title;
            IQueryable<Student> studentsQuery = _context.Enrollment.Where(x => x.CourseID == id).Select(x => x.Student);
            await _context.SaveChangesAsync();
            if (course == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(Fullname))
            {
                if (Fullname.Contains(" "))
                {
                    string[] names = Fullname.Split(" ");
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(names[0]) || x.LastName.Contains(names[1]) ||
                    x.FirstName.Contains(names[1]) || x.LastName.Contains(names[0]));
                }
                else
                {
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(Fullname) || x.LastName.Contains(Fullname));
                }
            }
            if (!string.IsNullOrEmpty(StudentID))
            {
                studentsQuery = studentsQuery.Where(x => x.StudentId.Contains(StudentID));
            }
            var StudentSearchVM = new StudentSearch
            {
                Students = await studentsQuery.ToListAsync()
            };

            return View(StudentSearchVM);

        }
  
        // GET: Students/EditPicture/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Student.Where(x => x.ID == id).Include(x => x.Courses).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Course.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            EditPictureStudent viewmodel = new EditPictureStudent
            {
                student = student,
                profilePictureName = student.ProfilePicture
            };

            return View(viewmodel);
        }

        // POST: Students/EditPicture/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(int id, EditPictureStudent viewmodel)
        {
            if (id != viewmodel.student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.profilePictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.student.ProfilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.student.ProfilePicture = viewmodel.profilePictureName;
                    }

                    _context.Update(viewmodel.student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.student.ID });
            }
            return View(viewmodel);
        }
        private string UploadedFile(EditPictureStudent viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.profilePictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.profilePictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.profilePictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
    }
}

    
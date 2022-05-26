using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rsweb10.Models;
using rsweb10.ViewModels;

namespace rsweb10.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly rsweb10Context _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public EnrollmentsController(rsweb10Context context)
        {
            _context = context;
        }

        // GET: Enrollments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int year, string searchString)
        {
            IQueryable<Enrollment> enrollmentsEE = _context.Enrollment.AsQueryable();
            IQueryable<int?> yeara = _context.Enrollment.OrderBy(m => m.Year).Select(m => m.Year).Distinct();

            if (!string.IsNullOrEmpty(searchString))
            {
                enrollmentsEE = enrollmentsEE.Where(x => x.Course.Title.Contains(searchString));
            }
            if (yeara != null)
            {
                enrollmentsEE = enrollmentsEE.Where(s => s.Year==year);
            }
            var VM = new EnrollmentSearch
            {
                yearsList = new SelectList(await yeara.ToListAsync()),
                enrollments=await enrollmentsEE.ToListAsync()
            };
            return View(VM);

    }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Enrollment == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CoursesID"] = new SelectList(_context.Course, "CourseID", "Title");
            ViewData["StudentID"] = new SelectList(_context.Set<Student>(), "ID", "FullName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentID,CourseID,StudentID,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CoursesID"] = new SelectList(_context.Course, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Set<Student>(), "ID", "FullName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enrollment == null)
            {
                return NotFound();
            }
         
            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "CourseID", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Set<Student>(), "ID", "FirstName", enrollment.StudentID);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentID,CourseID,StudentID,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
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
            ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "CourseID", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Set<Student>(), "ID", "FirstName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Enrollment == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Enrollment == null)
            {
                return Problem("Entity set 'rsweb10Context.Enrollment'  is null.");
            }
            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollment.Remove(enrollment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
          return (_context.Enrollment?.Any(e => e.EnrollmentID == id)).GetValueOrDefault();
        }

        // GET: Enrollments/StudentsEnrolledAtCourse/5
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<IActionResult> StudentsEnrolledAtCourse(int? id, string teacher, int year)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.CourseID == id);

            string[] names = teacher.Split(" ");
            var teacherModel = await _context.Teacher.FirstOrDefaultAsync(m => m.FirstName == names[0] && m.LastName == names[1]);
       
            ViewBag.Teacher = teacher;
            ViewBag.Course = course.Title;
            var enrollment = _context.Enrollment.Where(x => x.CourseID == id && (x.Course.FirstTeacherId == teacherModel.TeacherId || x.Course.SecondTeacherId == teacherModel.TeacherId))
            .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();
            IQueryable<int?> yearsQuery = _context.Enrollment.OrderBy(m => m.Year).Select(m => m.Year).Distinct();
            IQueryable<Enrollment> enrollmentQuery = enrollment.AsQueryable();
            if (year != null && year != 0)
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == year);
            }
            else
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == yearsQuery.Max());
            }

            if (enrollment == null)
            {
                return NotFound();
            }

            EnrollmentSearch viewmodel = new EnrollmentSearch
            {
                enrollments = await enrollmentQuery.ToListAsync(),
                yearsList = new SelectList(await yearsQuery.ToListAsync())
            };

            return View(viewmodel);
        }

        // GET: Enrollments/EditAsTeacher/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> EditAsTeacher(int? id, string teacher)
        {
            if (id == null)
            {
                return NotFound();
            }
            string[] names = teacher.Split(" ");
       
            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewBag.Teacher = teacher;
            ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Student, "ID", "FirstName", enrollment.StudentID);
            return View(enrollment);
        }


        // POST: Enrollments/EditAsTeacher/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> EditAsTeacher(int id, string teacher, [Bind("enrollmentId,courseId,studentId,semester,year,grade,seminalUrl,projectUrl,examPoints,seminalPoints,projectPoints,additionalPoints,finishDate")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return NotFound();
            }
            string temp = teacher;
            string[] names = teacher.Split(" ");
            var teacherModel = await _context.Teacher.FirstOrDefaultAsync(m => m.FirstName == names[0] && m.LastName == names[1]);
      
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("StudentsEnrolledAtCourse", new { id = enrollment.CourseID, teacher = temp, year = enrollment.Year });
            }
            ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Student, "ID", "FirstName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/StudentCourses/5
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> StudentCourses(int? id, int year , string searchString)
        {
            if (id == null)
            {
                return NotFound();
            }
          
            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.ID == id);

            ViewBag.Student = student.FirstName;

            IQueryable<Enrollment> enrollment = _context.Enrollment.Where(x => x.StudentID == id)
            .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();

            if (enrollment == null)
            {
                return NotFound();
            }

            IQueryable<int?> yeara = _context.Enrollment.OrderBy(m => m.Year).Select(m => m.Year).Distinct();

            if (!string.IsNullOrEmpty(searchString))
            {
                enrollment = enrollment.Where(x => x.Course.Title.Contains(searchString));
            }
            if (yeara != null)
            {
                enrollment = enrollment.Where(s => s.Year == year);
            }
            var VM = new EnrollmentSearch
            {
                yearsList = new SelectList(await yeara.ToListAsync()),
                enrollments = await enrollment.ToListAsync()
            };

           
     
            return View(VM);
        }


        // GET: Enrollments/EditAsStudent/5
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EditAsStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var enrollment = _context.Enrollment.Where(m => m.EnrollmentID == id).Include(x => x.Student).Include(x => x.Course).First();
            IQueryable<Enrollment> enrollmentQuery = _context.Enrollment.AsQueryable();
            enrollmentQuery = enrollmentQuery.Where(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            StudentEdit viewmodel = new StudentEdit
            {
                enrollment = await enrollmentQuery.Include(x => x.Student).Include(x => x.Course).FirstAsync(),
                seminalUrlName = enrollment.SeminalUrl
            };
            ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Student, "ID", "FirstName", enrollment.StudentID);
       
            return View(viewmodel);
        }

        // POST: Enrollments/EditAsStudent/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EditAsStudent(long id, StudentEdit viewmodel)
        {
            if (id != viewmodel.enrollment.EnrollmentID)
            {
                return NotFound();
            }
       
            if (ModelState.IsValid)
            {
                try
                {

                    if (viewmodel.seminalUrlFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.enrollment.SeminalUrl = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.enrollment.SeminalUrl = viewmodel.seminalUrlName;
                    }

                    _context.Update(viewmodel.enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(viewmodel.enrollment.EnrollmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("StudentCourses", new { id = viewmodel.enrollment.StudentID });
            }

            ViewData["CourseID"] = new SelectList(_context.Course, "CourseID", "Title", viewmodel.enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Student, "ID", "FirstName", viewmodel.enrollment.StudentID);
            return View(viewmodel);
        }

        private string UploadedFile(StudentEdit viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.seminalUrlFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.seminalUrlFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.seminalUrlFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
    }
}

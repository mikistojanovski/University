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
    public class CoursesController : Controller
    {
        private readonly rsweb10Context _context;

        public CoursesController(rsweb10Context context)
        {
            _context = context;
        }

        // GET: Courses
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string CourseProgramme, int CourseSemester, string searchString)
        {
            IQueryable<Course> courses = _context.Course.AsQueryable();
            IQueryable<string> programmQuery = _context.Course.OrderBy(m => m.Programme).Select(m => m.Programme).Distinct();
            IQueryable<int> semesterQuery = _context.Course.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(x => x.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(CourseProgramme))
            {
                courses = courses.Where(p => p.Programme == CourseProgramme);
            }
            if (CourseSemester!=null&&CourseSemester!=0)
            {
                courses = courses.Where(s => s.Semester == CourseSemester);
            }
           
            var VM = new CourseSearchViewModel
            {
                Programme = new SelectList(await programmQuery.ToListAsync()),
                Semesters = new SelectList(await semesterQuery.ToListAsync()),
                Course = await courses.Include(c=>c.FirstTeacher).Include(c=>c.SecondTeacher).ToListAsync(),
            };
            return View(VM);
        }

        // GET: Courses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            ViewData["Teachers"] = new SelectList(_context.Set<Teacher>(), "TeacherId", "FullName");
            ViewData["Students"] = new SelectList(_context.Set<Student>(), "ID", "FullName");

            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId,Students")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Teachers"] = new SelectList(_context.Set<Teacher>(), "TeacherId", "FullName");
            ViewData["Students"] = new SelectList(_context.Set<Student>(), "Id", "FullName");

            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _context.Course.Where(m => m.CourseID == id).Include(x => x.Students).First();
            IQueryable<Course> coursesQuery = _context.Course.AsQueryable();
            coursesQuery = coursesQuery.Where(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            var students = _context.Student.AsEnumerable();
            students = students.OrderBy(s => s.FullName);

            CourseStudentsEditViewModel viewmodel = new CourseStudentsEditViewModel
            {
                Course = await coursesQuery.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher).FirstAsync(),
                StudentList = new MultiSelectList(students, "Id", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.StudentID)
            };

            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);

            return View(viewmodel);

        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseStudentsEditViewModel viewmodel)
        {
            if (id != viewmodel.Course.CourseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Course);
                    await _context.SaveChangesAsync();

                    IEnumerable<int> listStudents = viewmodel.SelectedStudents;
                    if (listStudents != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentID) && s.CourseID == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);

                        IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentID) && s.CourseID == id).Select(s => s.StudentID);
                        IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
                        foreach (int studentID in newStudents)
                            _context.Enrollment.Add(new Enrollment { StudentID = studentID , CourseID = id });


                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => s.CourseID == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(viewmodel.Course.CourseID))
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

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Course == null)
            {
                return Problem("Entity set 'rsweb10Context.Course'  is null.");
            }
            var course = await _context.Course.FindAsync(id);
            if (course != null)
            {
                _context.Course.Remove(course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
          return (_context.Course?.Any(e => e.CourseID == id)).GetValueOrDefault();
        }


        // GET: Courses/CoursesTeaching/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> CoursesTeaching(int? id, string searchString, int CourseSemester, string CourseProgramm)
        {
            if (id == null)
            {
                return NotFound();
            }
           
            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            ViewBag.Message = teacher.FullName;
            IQueryable<Course> courses = _context.Course.Where(m => m.FirstTeacherId == id|| m.SecondTeacherId==id);
            await _context.SaveChangesAsync();
            if (teacher == null)
            {
                return NotFound();
            }
            IQueryable<int> semestersQuery = _context.Course.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            IQueryable<string> programmesQuery = _context.Course.OrderBy(m => m.Programme).Select(m => m.Programme).Distinct();
            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(x => x.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(CourseProgramm))
            {
                courses = courses.Where(p => p.Programme == CourseProgramm);
            }
            if (CourseSemester != null && CourseSemester != 0)
            {
                courses = courses.Where(s => s.Semester == CourseSemester);
            }
            var VM = new CourseSearchViewModel
            {
                Programme = new SelectList(await programmesQuery.ToListAsync()),
                Semesters = new SelectList(await semestersQuery.ToListAsync()),
                Course = await courses.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher).ToListAsync()
            };

            return View(VM);
        }


    }
}

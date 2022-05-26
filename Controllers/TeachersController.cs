#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class TeachersController : Controller
    {
        private readonly rsweb10Context _context;
        private UserManager<rsweb10User> _userManager;
        public TeachersController(rsweb10Context context, UserManager<rsweb10User> userManager)
        {
            _context = context;

            _userManager = userManager;
        }

        // GET: Teachers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchString, string TeacherDegree, string TeacherAR)
        {
            IQueryable<Teacher> teachers = _context.Teacher.AsQueryable();
            IQueryable<string> degreeQuery = _context.Teacher.OrderBy(m => m.Degree).Select(m => m.Degree).Distinct();
            IQueryable<string> ARQuery = _context.Teacher.OrderBy(m => m.AcademicRank).Select(m => m.AcademicRank).Distinct();
            if (!string.IsNullOrEmpty(searchString))
            {
                if (searchString.Contains(" "))
                {
                    string[] names = searchString.Split(" ");
                    teachers = teachers.Where(x => x.FirstName.Contains(names[0]) || x.LastName.Contains(names[1]) ||
                    x.FirstName.Contains(names[1]) || x.LastName.Contains(names[0]));
                }
                else
                {
                    teachers = teachers.Where(x => x.FirstName.Contains(searchString) || x.LastName.Contains(searchString));
                }
            }
            if (!string.IsNullOrEmpty(TeacherDegree))
            {
                teachers = teachers.Where(x => x.Degree == TeacherDegree);
            }
            if (!string.IsNullOrEmpty(TeacherAR))
            {
                teachers = teachers.Where(a => a.AcademicRank == TeacherAR);
            }
          
            var TeacherFilterVM = new TeacherSearchViewModel
            {
                Degree = new SelectList(await degreeQuery.ToListAsync()),
                AcademicRank = new SelectList(await ARQuery.ToListAsync()),
                Teacher = await teachers.ToListAsync()
            };
            return View(TeacherFilterVM);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Teacher == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            EditPictureTeacher viewmodel = new EditPictureTeacher
            {
                teacher = teacher,
                profilePictureName = teacher.ProfilePicture
            };
            return View(viewmodel);
        }

        // GET: Teachers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeacherId,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                var User = new rsweb10User();
                User.Email = teacher.FirstName.ToLower() + teacher.LastName.ToLower() + "@rsweb10.com";
                User.UserName = teacher.FirstName.ToLower() + "." + teacher.LastName.ToLower();
                string userPWD = "Teacher123";
                IdentityResult chkUser = await _userManager.CreateAsync(User, userPWD);
                if (chkUser.Succeeded) { var result1 = await _userManager.AddToRoleAsync(User, "Teacher"); }
                teacher.userIdentityId = User.Id;
                _context.Add(teacher);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Teacher == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeacherId,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate,ProfilePicture")] Teacher teacher)
        {
            if (id != teacher.TeacherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.TeacherId))
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
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Teacher == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Teacher == null)
            {
                return Problem("Entity set 'rsweb10Context.Teacher'  is null.");
            }
            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher != null)
            {
                _context.Teacher.Remove(teacher);
            }
            IQueryable<Course> courses = _context.Course.AsQueryable();
            IQueryable<Course> courses1 = courses.Where(x => x.FirstTeacherId == teacher.TeacherId);
            IQueryable<Course> courses2 = courses.Where(x => x.SecondTeacherId == teacher.TeacherId);
            foreach (var course in courses1)
            {
                course.FirstTeacherId = null;
            }
            foreach (var course in courses2)
            {
                course.SecondTeacherId = null;
            }
            _context.Teacher.Remove(teacher);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
          return (_context.Teacher?.Any(e => e.TeacherId == id)).GetValueOrDefault();
        }

        // GET: Teachers/EditPicture/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = _context.Teacher.Where(x => x.TeacherId == id).First();
            if (teacher == null)
            {
                return NotFound();
            }

            EditPictureTeacher viewmodel = new EditPictureTeacher
            {
                teacher = teacher,
                profilePictureName = teacher.ProfilePicture
            };

            await _context.SaveChangesAsync();
            return View(viewmodel);
        }

        // POST: Teachers/EditPicture/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPicture(int id, EditPictureTeacher viewmodel)
        {
            if (id != viewmodel.teacher.TeacherId)
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
                        viewmodel.teacher.ProfilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.teacher.ProfilePicture = viewmodel.profilePictureName;
                    }

                    _context.Update(viewmodel.teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(viewmodel.teacher.TeacherId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.teacher.TeacherId });
            }
            return View(viewmodel);
        }
        private string UploadedFile(EditPictureTeacher viewmodel)
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

                using var stream = new FileStream(fileNameWithPath, FileMode.Create);
                viewmodel.profilePictureFile.CopyTo(stream);
            }
            return uniqueFileName;
        }
    }
}

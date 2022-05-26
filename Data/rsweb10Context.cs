using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rsweb10.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rsweb10.Areas.Identity.Data;


namespace rsweb10.Models
{
    public class rsweb10Context : IdentityDbContext<rsweb10User>
    {
        public rsweb10Context (DbContextOptions<rsweb10Context> options)
            : base(options)
        {
        }

        public DbSet<rsweb10.Models.Course>? Course { get; set; }

        public DbSet<rsweb10.Models.Enrollment>? Enrollment { get; set; }

        public DbSet<rsweb10.Models.Student>? Student { get; set; }

        public DbSet<rsweb10.Models.Teacher>? Teacher { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
           
            builder.Entity<Enrollment>()
       .HasOne<Student>(p => p.Student)
       .WithMany(p => p.Courses)
       .HasForeignKey(p => p.StudentID);


            builder.Entity<Enrollment>()
                   .HasOne<Course>(p => p.Course)
                   .WithMany(p => p.Students)
                   .HasForeignKey(p => p.CourseID);

            builder.Entity<Course>()
                .HasOne<Teacher>(p => p.FirstTeacher)
                .WithMany(p => p.Course1)
                .HasForeignKey(p => p.FirstTeacherId);

            builder.Entity<Course>()
                .HasOne<Teacher>(p => p.SecondTeacher)
                .WithMany(p => p.Course2)
                .HasForeignKey(p => p.SecondTeacherId);

            base.OnModelCreating(builder);
        }
    }
}

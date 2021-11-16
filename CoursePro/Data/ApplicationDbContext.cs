using System;
using System.Collections.Generic;
using System.Text;
using CoursePro.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoursePro.Data
{
    public class ApplicationDbContext : IdentityDbContext<CPUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Classification> Classifications { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TextBook> TextBooks { get; set; }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursePro.Data;
using CoursePro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

public static class DataUtility
{

    //Get classification Ids
    private static int freshman;
    private static int sophpmore;
    private static int junior;
    private static int senior;

    public static string GetConnectionString(IConfiguration configuration)
    {
        //The default connection string will come from appSettings like usual
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        //It will be automatically overwritten if we are running on Heroku
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
    }

    public static string BuildConnectionString(string databaseUrl)
    {
        //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Prefer,
            TrustServerCertificate = true
        };
        return builder.ToString();
    }

    public static async Task ManageDataAsync(IHost host)
    {
        using var svcScope = host.Services.CreateScope();
        var svcProvider = svcScope.ServiceProvider;
        //Service: An instance of RoleManager
        var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
        ////Service: An instance of RoleManager
        //var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //Service: An instance of the UserManager
        var userManagerSvc = svcProvider.GetRequiredService<UserManager<CPUser>>();
        //Migration: This is the programmatic equivalent to Update-Database
        await dbContextSvc.Database.MigrateAsync();

        //Custom Bug Tracker Seed Methods
        await SeedClassificationsAsync(dbContextSvc);
        await SeedSchedulesAsync(dbContextSvc);
        await SeedTextBooksAsync(dbContextSvc);
        await SeedCoursesAsync(dbContextSvc);
        await SeedCPUsersAsync(userManagerSvc);
    }


    public static async Task SeedClassificationsAsync(ApplicationDbContext context)
    {
        try
        {
            IList<Classification> defaultClassifications = new List<Classification>() {
                    new Classification() { Name = nameof(CPClassification.Freshman) },
                    new Classification() { Name = nameof(CPClassification.Sophomore) },
                    new Classification() { Name = nameof(CPClassification.Junior) },
                    new Classification() { Name = nameof(CPClassification.Senior)  }
                };

            var dbClassifications = context.Classifications.Select(c => c.Name).ToList();
            await context.Classifications.AddRangeAsync(defaultClassifications.Where(c => !dbClassifications.Contains(c.Name)));
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding Classifications.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }
    public static async Task SeedSchedulesAsync(ApplicationDbContext context)
    {
        try
        {
            IList<Schedule> defaultSchedules = new List<Schedule>() {
                    new Schedule() { DaysOffered = nameof(CPSchedule.M) },
                    new Schedule() { DaysOffered = nameof(CPSchedule.T) },
                    new Schedule() { DaysOffered = nameof(CPSchedule.W) },
                    new Schedule() { DaysOffered = nameof(CPSchedule.TTH) },
                    new Schedule() { DaysOffered = nameof(CPSchedule.F) },
                    new Schedule() { DaysOffered = nameof(CPSchedule.MWF) },
                    new Schedule() { DaysOffered = nameof(CPSchedule.TTH) },
                    new Schedule() { DaysOffered = nameof(CPSchedule.MW) }
                };

            var dbSchedules = context.Schedules.Select(c => c.DaysOffered).ToList();
            await context.Schedules.AddRangeAsync(defaultSchedules.Where(c => !dbSchedules.Contains(c.DaysOffered)));
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding default Schedules.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }
    public static async Task SeedTextBooksAsync(ApplicationDbContext context)
    {

        try
        {
            IList<TextBook> textbooks = new List<TextBook>() {

                                new TextBook() {
                                    Title = "The C-O-D-E",
                                    Description = "This book is needed for every course offered.",
                                },
                                 new TextBook() {
                                    Title = "Learning the Code",
                                    Description = "This is a text book for sophomores.",
                                },
                                  new TextBook() {
                                    Title = "Ok Then Mr Coder Man",
                                    Description = "All juniors need to have this text book",
                                },
                                   new TextBook() {
                                    Title = "So... You Think You Can Code",
                                    Description = "A senior text book",
                                }
                };


            var dbTextBooks = context.TextBooks.Select(c => c.Title).ToList();
            await context.TextBooks.AddRangeAsync(textbooks.Where(c => !dbTextBooks.Contains(c.Title)));
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding TextBooks.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }
    public static async Task SeedCoursesAsync(ApplicationDbContext context)
    {
        //Get classification Ids
        freshman = context.Classifications.FirstOrDefault(c => c.Name == nameof(CPClassification.Freshman)).Id;
        sophpmore = context.Classifications.FirstOrDefault(c => c.Name == nameof(CPClassification.Sophomore)).Id;
        junior = context.Classifications.FirstOrDefault(c => c.Name == nameof(CPClassification.Junior)).Id;
        senior = context.Classifications.FirstOrDefault(c => c.Name == nameof(CPClassification.Senior)).Id;

        //Get schedule Ids
        int monwedfri = context.Schedules.FirstOrDefault(s => s.DaysOffered == nameof(CPSchedule.MWF)).Id;
        int tuethur = context.Schedules.FirstOrDefault(s => s.DaysOffered == nameof(CPSchedule.TTH)).Id;
        int monwed = context.Schedules.FirstOrDefault(s => s.DaysOffered == nameof(CPSchedule.MW)).Id;
        int wednesday = context.Schedules.FirstOrDefault(s => s.DaysOffered == nameof(CPSchedule.W)).Id;

        try
        {
            IList<Course> courses = new List<Course>() {
                     new Course()
                     {
                         Name = "Real Coding 101",
                         Description="Do you want to code?" ,
                         ClassificationId = freshman,
                         ScheduleId = monwedfri,
                     },
                     new Course()
                     {
                         Name = "Real Coding 201",
                         Description="Can you code?" ,
                         ClassificationId = sophpmore,
                         ScheduleId = tuethur,
                     },
                     new Course()
                     {
                         Name = "Real Coding 301",
                         Description="You can code!" ,
                         ClassificationId = junior,
                         ScheduleId = monwed,
                     },
                     new Course()
                     {
                         Name = "Real Coding 401",
                         Description="Show me you can code!" ,
                         ClassificationId = senior,
                         ScheduleId = wednesday,
                     }
                };

            var dbCourses = context.Courses.Select(c => c.Name).ToList();
            await context.Courses.AddRangeAsync(courses.Where(c => !dbCourses.Contains(c.Name)));
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding Courses.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }
    public static async Task SeedCPUsersAsync(UserManager<CPUser> userManager)
    {

        var defaultUser = new CPUser
        {
            UserName = "cpuser1@coursespro.com",
            Email = "cpuser1@coursespro.com",
            FirstName = "Alpha",
            LastName = "Appuser",
            ClassificationId = freshman,
            EmailConfirmed = true
        };
        try
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Abc&123!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding CPUser1.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }

        //Seed CPUser2
        defaultUser = new CPUser
        {
            UserName = "cpuser2@coursespro.com",
            Email = "cpuser2@coursespro.com",
            FirstName = "Beta",
            LastName = "Appuser",
            ClassificationId = sophpmore,
            EmailConfirmed = true
        };
        try
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Abc&123!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding Default Admin User.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }


        //Seed Default ProjectManager1 User
        defaultUser = new CPUser
        {
            UserName = "cpuser3@coursespro.com",
            Email = "cpuser3@coursespro.com",
            FirstName = "Charlie",
            LastName = "Appuser",
            ClassificationId = junior,
            EmailConfirmed = true
        };
        try
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Abc&123!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding CPUser3.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }


        //Seed Default ProjectManager2 User
        defaultUser = new CPUser
        {
            UserName = "cpuser4@coursespro.com",
            Email = "cpuser4@coursespro.com",
            FirstName = "Delta",
            LastName = "Appuser",
            ClassificationId = senior,
            EmailConfirmed = true
        };
        try
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Abc&123!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding CPUser4.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoursePro.Models
{
    public class Course
    {

        public int Id { get; set; } //PK
        public int ScheduleId { get; set; } //FK
        public int ClassificationId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }


        // Navigational Property

        public virtual Schedule Schedule { get; set; }
        public virtual Classification Classification { get; set; }
        public ICollection<CPUser> Members { get; set; } = new HashSet<CPUser>();
        public ICollection<TextBook> TextBooks { get; set; } = new HashSet<TextBook>();


    }
}

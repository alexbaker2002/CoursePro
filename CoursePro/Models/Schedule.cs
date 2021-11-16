using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoursePro.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        [Required]
        public string DaysOffered { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }


        //Navigation
        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();

    }
}

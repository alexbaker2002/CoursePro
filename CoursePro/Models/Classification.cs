using System;
using System.ComponentModel.DataAnnotations;

namespace CoursePro.Models
{
    public class Classification
    {
        public int Id { get; set; } // PK
        [Required]
        public string Name { get; set; }

    }
}

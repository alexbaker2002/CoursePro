using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace CoursePro.Models
{
    public class TextBook
    {
        public int Id { get; set; }//pk
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        [Display(Name = "User Image")]
        public IFormFile ImageFileName { get; set; } // file on computer 
        public byte[] ImageFileData { get; set; } // file split up into bytes and added into an array // byte[] ImageData
        public string ImageContentType { get; set; } // store the image type so it can be rendered


        // Navigational Props
        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
        public ICollection<CPUser> Owners { get; set; } = new HashSet<CPUser>();

    }
}

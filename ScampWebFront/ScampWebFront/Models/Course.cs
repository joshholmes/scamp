using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScampWebFront.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } 
        public List<Student> Students { get; set; }
    }
}
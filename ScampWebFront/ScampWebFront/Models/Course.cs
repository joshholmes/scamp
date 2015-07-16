using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScampWebFront.Models
{
    public class Course
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
    }
}
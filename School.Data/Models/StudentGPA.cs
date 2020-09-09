using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace School.Data.Models
{
    public class StudentGPA
    {
        public int studentId { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public decimal? gpa { get; set; }

    }
}

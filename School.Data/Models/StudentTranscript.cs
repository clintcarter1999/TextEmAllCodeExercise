using System;
using System.Collections.Generic;
using System.Text;

namespace School.Data.Models
{
    public class StudentTranscript
    {
        public int studentId { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public decimal? gpa { get; set; }

        public List<StudentTranscriptGrade> grades { get; set; }

    }
}

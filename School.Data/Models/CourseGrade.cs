using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace School.Data.Models
{
    public class CourseGrade
    {
        public int gradeId { get; set; }

        [Required]
        public int studentId { get; set; }

        [Required]
        public int courseId { get; set; }

        [Required, Range(0, 4)]
        public decimal? grade { get; set; }
    }
}

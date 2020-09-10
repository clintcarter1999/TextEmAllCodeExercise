
using System;
using System.Collections.Generic;
using System.Text;

namespace School.Data.Models
{
    /// <summary>
    /// This class is used to model the "grades" collection of the Student Transcript 
    /// as shown in Challenge 1:
    ///
    /// {
    ///  "studentId":2,
    ///  "firstName":"Gytis",
    ///  "lastName":"Barzdukas",
    ///  "gpa":3.8,
    ///  "grades":[
    ///     {
    ///        "courseId":2021,
    ///        "title":"Composition",
    ///        "credits":3,
    ///        "grade":4
    ///     },
    ///     {
    ///        "courseId":2030,
    ///        "title":"Poetry",
    ///        "credits":2,
    ///        "grade":3.5
    ///     }
    ///  ]
    /// }
    /// </summary>
    public class StudentTranscriptGrade
    {
        public int courseId { get; set; }

        public string title { get; set; }

        public int credits { get; set; }

        public decimal? grade { get; set; }

    }
}

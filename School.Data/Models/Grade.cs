
using System;
using System.Collections.Generic;
using System.Text;

namespace School.Data.Models
{
    public class Grade
    {
        public int courseId { get; set; }

        public string title { get; set; }

        public int credits { get; set; }

        public decimal? grade { get; set; }

    }
}

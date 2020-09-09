using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Data.Models
{
    public partial class CourseInstructor
    {
        [Key]
        [Column("CourseID")]
        public int CourseId { get; set; }

        [Key]
        [Column("PersonID")]
        public int PersonId { get; set; }

        [ForeignKey(nameof(CourseId))]
        [InverseProperty("CourseInstructor")]
        public virtual Course Course { get; set; }

        [ForeignKey(nameof(PersonId))]
        [InverseProperty("CourseInstructor")]
        public virtual Person Person { get; set; }
    }
}

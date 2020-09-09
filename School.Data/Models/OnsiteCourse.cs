using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Data.Models
{
    public partial class OnsiteCourse
    {
        [Key]
        [Column("CourseID")]
        public int CourseId { get; set; }

        [Required]
        [StringLength(50)]
        public string Location { get; set; }

        [Required]
        [StringLength(50)]
        public string Days { get; set; }

        public TimeSpan Time { get; set; }

        [ForeignKey(nameof(CourseId))]
        [InverseProperty("OnsiteCourse")]
        public virtual Course Course { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Data.Models
{
    public partial class OnlineCourse
    {
        [Key]
        [Column("CourseID")]
        public int CourseId { get; set; }

        [Required]
        [Column("URL")]
        [StringLength(100)]
        public string Url { get; set; }

        [ForeignKey(nameof(CourseId))]
        [InverseProperty("OnlineCourse")]
        public virtual Course Course { get; set; }
    }
}

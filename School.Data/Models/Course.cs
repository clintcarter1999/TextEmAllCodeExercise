using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Data.Models
{
    public partial class Course
    {
        public Course()
        {
            CourseInstructor = new HashSet<CourseInstructor>();
            StudentGrade = new HashSet<StudentGrade>();
        }

        [Key]
        [Column("CourseID")]
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public int Credits { get; set; }

        [Column("DepartmentID")]
        public int DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        [InverseProperty("Course")]
        public virtual Department Department { get; set; }

        [InverseProperty("Course")]
        public virtual OnlineCourse OnlineCourse { get; set; }

        [InverseProperty("Course")]
        public virtual OnsiteCourse OnsiteCourse { get; set; }

        [InverseProperty("Course")]
        public virtual ICollection<CourseInstructor> CourseInstructor { get; set; }

        [InverseProperty("Course")]
        public virtual ICollection<StudentGrade> StudentGrade { get; set; }
    }
}

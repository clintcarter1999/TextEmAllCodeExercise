using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Data.Models
{
    public partial class OfficeAssignment
    {
        [Key]
        [Column("InstructorID")]
        public int InstructorId { get; set; }

        [Required]
        [StringLength(50)]
        public string Location { get; set; }

        [Required]
        public byte[] Timestamp { get; set; }

        [ForeignKey(nameof(InstructorId))]
        [InverseProperty(nameof(Person.OfficeAssignment))]
        public virtual Person Instructor { get; set; }
    }
}

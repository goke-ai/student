using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Goke.Students.Shared
{
    public class StudentTarget
    {
        [Required]
        public int TargetId { get; set; } = 2;
        public Target Target => Target.Targets.FirstOrDefault(f => f.Id == TargetId);

        [Required]
        public int CapabilityId { get; set; } = 1;
        public Grade Capability => Grade.Grades.FirstOrDefault(f => f.Id == CapabilityId);

        [Required]
        public int SemesterId { get; set; } = 3;
        public Semester SemesterPerYear => Semester.Semesters.FirstOrDefault(f => f.Id == SemesterId);

        public int AverageCoursePerSemester { get; set; } = 4;

        public int MaximumCourseUnit { get; set; } = 5;

        public override string ToString()
        {
            return $" TargetId:{TargetId}, CapabilityId:{CapabilityId}, SemesterId:{SemesterId}, AverageCoursePerSemester:{AverageCoursePerSemester}, MaximumCourseUnit:{MaximumCourseUnit} ";
        }

        public List<Course> Courses { get; set; }


    }
}

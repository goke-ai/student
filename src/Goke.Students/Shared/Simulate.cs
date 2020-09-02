using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Goke.Students.Shared
{
    public class Simulate
    {
        public static List<Course> CreateCourses(StudentTarget studentTarget)
        {
            Random rnd = new Random();

            List<Course> courses = new List<Course>();

            if (studentTarget?.AverageCoursePerSemester > 0)
            {
                for (int s = 0; s < studentTarget.SemesterPerYear.Value; s++)
                {
                    for (int c = 0; c < studentTarget.AverageCoursePerSemester; c++)
                    {
                        var code = $"X{s + 1}0{c + 1}";
                        var course = new Course
                        {
                            Semester = s+1,
                            Code = code,
                            Title = $"Title of {code}",
                            Unit = rnd.Next(1, studentTarget.MaximumCourseUnit+1),
                        };

                        courses.Add(course);
                    }
                }
            }

            return courses;
        }

        public static List<Course> GradeCourses(List<Course> courses, List<Grade> grades)
        {
            Random rnd = new Random();
            var gradeIds = new List<int> { -1 };
            gradeIds.AddRange( grades.Select(x => x.Id));

            foreach (var c in courses)
            {
                var i = rnd.Next(0, grades.Count);
                c.GradeId = gradeIds[i];
            }

            return courses;
        }
    }
}

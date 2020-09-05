using Goke.Optimization;
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
            gradeIds.AddRange(grades.Select(x => x.Id));

            foreach (var c in courses)
            {
                var i = rnd.Next(0, grades.Count);
                c.GradeId = gradeIds[i];
            }

            return courses;
        }

        public static (List<Course> courses, List<Course>[] alternativeCourses) OptimalGradeCourses(double target, List<Course> courses, List<Grade> grades)
        {
            float[] gradePoints = grades.Where(w => w.Point > 0).Select(s => s.Point).OrderBy(o => o).ToArray();
            var unit = courses.Select(s => s.Unit).ToArray();
            var cgpa = target;

            Func<double[], double> Fn = (x) =>
            {
                var U = unit;
                var CGPA = cgpa;

                double sum1 = 0;
                double sum2 = 0;
                for (int i = 0; i < x.Length; i++)
                {
                    sum1 += x[i] * U[i];
                    sum2 += CGPA * U[i];
                }
                return Math.Abs(sum1 - sum2);
            };

            var nsa = 30;
            var max_iter = 1000;

            double lb = gradePoints[1];
            double ub = gradePoints.Last();
            int dim = unit.Length;

            int soln = 10;

            double[] bFlameScores = new double[soln];
            double[][] bFlamesPositions = new double[soln][];
            double[][] convergenceCurves = new double[soln][];

            for (int i = 0; i < soln; i++)
            {
                (bFlameScores[i], bFlamesPositions[i], convergenceCurves[i]) = MFO.Search(nsa, dim, ub, lb, max_iter, Fn);
            }
            double mean = 0.0;
            double sum = 0.0;
            for (int i = 0; i < bFlameScores.Length; i++)
            {
                sum += bFlameScores[i];
            }
            mean = sum / bFlameScores.Length;

            double std = 0.0;

            // Console.WriteLine($"Mean : {mean}");
            // Console.WriteLine($"Standard deviation : {std}");

            double[][] rFlamesPositions = new double[soln][];

            for (int i = 0; i < bFlamesPositions.Length; i++)
            {
                double sumG = 0;
                double sumU = 0;

                rFlamesPositions[i] = new double[dim + 1];
                for (int j = 0; j < dim; j++)
                {
                    // find index
                    var v = bFlamesPositions[i][j];
                    var k = gradePoints.ToList().Find(f => (v - f) < 0.35);
                    rFlamesPositions[i][j] = k;

                    sumG += (k * unit[j]);
                    sumU += unit[j];

                    // Console.Write($"{k},  ");
                }
                rFlamesPositions[i][dim] = sumG / sumU;
                // Console.Write($": {rFlamesPositions[i][dim]}");

                // Console.WriteLine();
            }
            // Console.WriteLine("=========================");

            var gpSolutions = rFlamesPositions.Distinct().Where(w => w[dim] >= cgpa).OrderBy(o => o[dim]).ToList();
            //foreach (var tt in gpSolutions)
            //{
            //    foreach (var t in tt)
            //    {
            //        // Console.Write($"{t}, ");
            //    }
            //    // Console.WriteLine();
            //}

            var selectedGPs = gpSolutions.First();
            courses = SetCourseGradePoints(courses, grades, selectedGPs);

            List<Course>[] alternativeCourses = new List<Course>[gpSolutions.Count() - 1];
            for (int i = 1; i < gpSolutions.Count(); i++)
            {
                var list = new List<Course>();
                foreach (var c in courses)
                {
                    list.Add(new Course { 
                    Code = c.Code,
                    GradeId = c.GradeId,
                    Semester = c.Semester,
                    Title = c.Title,
                    Unit = c.Unit,
                    Id = c.Id,
                    });
                }

                list = SetCourseGradePoints(list, grades, gpSolutions[i]);
                alternativeCourses[i - 1] = list;
            }

            return (courses, alternativeCourses);
        }

        private static List<Course> SetCourseGradePoints(List<Course> courses, List<Grade> grades, double[] selectedGPs)
        {
            for (int i = 0; i < courses.Count; i++)
            {
                courses[i].GradeId = grades.First(f => f.Point == selectedGPs[i]).Id;
            }

            return courses;
        }
    }
}

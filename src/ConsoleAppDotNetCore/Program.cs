using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Goke.Optimization;
using Goke.Students.Shared;

namespace ConsoleAppDotNetCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // SimpleLP();
            (double ObjValue, double X, double Y) = Ortools.SimpleLpProgram();
            Console.WriteLine($"\n=======\n{ObjValue}: {X}, {X}\n");

            //
            double[] gradePoints = { 1, 2, 3, 4, 5 };
            var unit = new double[] { 1, 3, 4, 2, 5 };
            var cgpa = 4;

            Ortools.CGPALpProgram(unit, cgpa);

            //
            StudentTarget studentTarget = new StudentTarget();
            studentTarget.Courses = await Simulate.CreateCoursesAsync(studentTarget);
            // studentTarget.Courses = Simulate.GradeCourses(studentTarget.Courses, Grade.Grades);
            var optimal = await Simulate.OptimalGradeCoursesAsync(studentTarget.Target.Point, studentTarget.Courses, Grade.Grades);

            Console.WriteLine("=========");

            foreach (var x in optimal.courses)
            {
                Console.Write($"{x.Grade.Point}, ");
            }

            Console.WriteLine();
            Console.WriteLine("=========");

            foreach (var aCourses in optimal.alternativeCourses)
            {
                foreach (var x in aCourses)
                {
                    Console.Write($"{x.Grade.Point}, ");
                }
                Console.WriteLine();
            }
        }

        /*
        private static void SimpleLP()
        {
            // [START solver]
            // Create the linear solver with the GLOP backend.
            Solver solver = Solver.CreateSolver("SimpleLpProgram", "GLOP");
            // [END solver]

            // [START variables]
            // Create the variables x and y.
            Variable x = solver.MakeNumVar(0.0, 1.0, "x");
            Variable y = solver.MakeNumVar(0.0, 2.0, "y");

            Console.WriteLine("Number of variables = " + solver.NumVariables());
            // [END variables]

            // [START constraints]
            // Create a linear constraint, 0 <= x + y <= 2.
            Constraint ct = solver.MakeConstraint(0.0, 2.0, "ct");
            ct.SetCoefficient(x, 1);
            ct.SetCoefficient(y, 1);

            Console.WriteLine("Number of constraints = " + solver.NumConstraints());
            // [END constraints]

            // [START objective]
            // Create the objective function, 3 * x + y.
            Objective objective = solver.Objective();
            objective.SetCoefficient(x, 3);
            objective.SetCoefficient(y, 1);
            objective.SetMaximization();
            // [END objective]

            // [START solve]
            solver.Solve();
            // [END solve]

            // [START print_solution]
            Console.WriteLine("Solution:");
            Console.WriteLine("Objective value = " + solver.Objective().Value());
            Console.WriteLine("x = " + x.SolutionValue());
            Console.WriteLine("y = " + y.SolutionValue());
            // [END print_solution];
        }
        */

    }
}

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Goke.Optimization;


namespace ConsoleAppDotNetCore
{
    class Program
    {
        static void Main(string[] args)
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

            double lb = 1;
            double ub = 5;
            int dim = 5;

            int soln = 20;

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

            Console.WriteLine($"Mean : {mean}");
            Console.WriteLine($"Standard deviation : {std}");

            double[][] rFlamesPositions = new double[soln][];

            for (int i = 0; i < bFlamesPositions.Length; i++)
            {
                double sumG = 0;
                double sumU = 0;

                rFlamesPositions[i] = new double[dim+1];
                for (int j = 0; j < dim; j++)
                {
                    // find index
                    var v = bFlamesPositions[i][j];
                    var k = gradePoints.ToList().Find(f => (v - f) < 0.35);
                    rFlamesPositions[i][j] = k;

                    sumG += (k * unit[j]);
                    sumU += unit[j];

                    Console.Write($"{k},  ");
                }
                rFlamesPositions[i][dim] = sumG / sumU;
                Console.Write($": {rFlamesPositions[i][dim]}");

                Console.WriteLine();
            }
            Console.WriteLine("=========================");


            var sorted = rFlamesPositions.Distinct().Where(w => w[dim] >= cgpa).OrderBy(o => o[dim]);
            foreach (var tt in sorted)
            {
                foreach (var t in tt)
                {
                    Console.Write($"{t}, ");
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

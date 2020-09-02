using System;
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

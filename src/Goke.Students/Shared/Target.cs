using System.Collections.Generic;

namespace Goke.Students.Shared
{
    public class Target 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Point { get; set; }

        public static List<Target> Targets
        {
            get
            {
                return new List<Target> {
                    new Target { Id=1, Name="First Class", Point=4.5 },
                    new Target { Id=2, Name="Second Class Upper", Point=4.0 },
                    new Target { Id=3, Name="Second Class Lower", Point=3.5 },
                    new Target { Id=4, Name="Third Class", Point=2.4 },
                    new Target { Id=5, Name="Pass", Point=1.0 },
                };
            }
        }
    }
}
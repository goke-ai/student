using System.Collections.Generic;

namespace Goke.Students.Shared
{
    public class Year
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public static List<Year> Years
        {
            get
            {
                return new List<Year> {
                    new Year { Id=1, Name="2", Value=2 },
                    new Year { Id=2, Name="3", Value=3 },
                    new Year { Id=3, Name="4", Value=4 },
                };
            }
        }
    }
}
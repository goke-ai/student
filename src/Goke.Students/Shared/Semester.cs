using System.Collections.Generic;

namespace Goke.Students.Shared
{
    public class Semester
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public static List<Semester> Semesters
        {
            get
            {
                return new List<Semester> {
                    new Semester { Id=1, Name="1", Value=1 },
                    new Semester { Id=2, Name="2", Value=2 },
                    new Semester { Id=3, Name="3", Value=3 },
                    new Semester { Id=4, Name="4", Value=4 },
                    new Semester { Id=5, Name="5", Value=5 },
                    new Semester { Id=6, Name="6", Value=6 },
                    new Semester { Id=7, Name="7", Value=7 },
                    new Semester { Id=8, Name="8", Value=8 },
                };
            }
        }
    }
}
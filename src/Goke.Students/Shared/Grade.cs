using System.Collections.Generic;

namespace Goke.Students.Shared
{
    public class Grade
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Point { get; set; }

        public static List<Grade> Grades
        {
            get
            {
                return new List<Grade> {
                    new Grade { Id=1, Name="A", Point=5.0f },
                    new Grade { Id=2, Name="B", Point=4.0f },
                    new Grade { Id=3, Name="C", Point=3.0f },
                    new Grade { Id=4, Name="D", Point=2.0f },
                    new Grade { Id=5, Name="E", Point=1.0f },
                    new Grade { Id=6, Name="F", Point=0.0f },
                    new Grade { Id=-1, Name="", Point=-1.0f },
                };
            }
        }
    }
}
using System.Linq;

namespace Goke.Students.Shared
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public float Unit { get; set; }
        public int GradeId { get; set; }

        public Grade Grade => Grade.Grades.FirstOrDefault(f => f.Id == GradeId);

        public int Semester { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Semester: {Semester}, Code: {Code}, Title: {Title}, Unit: {Unit}, GradeId: {GradeId}, Grade: {Grade?.Point}";
        }
    }
}
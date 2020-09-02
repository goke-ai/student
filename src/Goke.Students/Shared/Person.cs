using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goke.Students.Shared
{
    public enum Gender { Male, Female }

    public class Country
    {
        public Country()
        {

        }

        public Country(string name, string postCode)
        {
            Name = name;
            PostCode = postCode;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PostCode { get; set; }


        public ICollection<Person> People { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Person Number")]
        public string PersonNumber { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Please provide First name")]
        [Display(Name = "First name", Description = "What is your name", Prompt = "Enter your First name")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name ="Last name")]
        public string Lastname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Title { get; set; }

        [Required]
        public string Address { get; set; }

        [Range(20,65)]
        public int RetirementAge { get; set; }

        [Required]
        [Range(300.20, 100000.50, ErrorMessage = "Salary invalid (300.20-100000.50).")]
        public double Salary { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "This form disallows uncertified engineer.")]
        public bool IsCertifiedEngineer { get; set; }

        [Required(ErrorMessage = "Please provide Birth Date")]
        [Display(Name ="Birth Date", Description ="What date you were born")]
        public DateTime? BirthDate { get; set; }

        public int? CountryId { get; set; }
        public Country Country { get; set; }

        public Gender Gender { get; set; }

        [Required]
        public string LastUser { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }
    }
}

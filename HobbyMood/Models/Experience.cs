// using DataAnnotations necessary to indicate key attributes
using System.ComponentModel.DataAnnotations;

namespace HobbyMood.Models
{
    public class Experience
    {
        // experience id
        public int ExperienceId { get; set; }

        // experience name
        public required string ExperienceName { get; set; }


        // hobby id (FK)
        public int HobbyId { get; set; }


        // experience cost in dollars
        public decimal ExperienceCost { get; set; }


        // hours spent in experience
        public decimal DurationinHours { get; set; }


        // date of experience

        public DateTime ExperienceDate { get; set; }


        // location of experience

        public string? ExperienceLocation { get; set; }

        // foreign key to the hobbies table -- 1 hobby to many experiences
        public virtual Hobby? Hobby { get; set; }


    }

    public class ExperienceDto
    { 

        // experience id
        public int ExperienceId { get; set; }

    
        // experience name 
        public string? ExperienceName { get; set; }


        // the hobby name comes from the hobbies table
        public string? HobbyName { get; set; }

        // the hobby id

        public int HobbyId { get; set; }

        // the experience cost 
        public decimal ExperienceCost { get; set; }


        // duration in hours 
        public decimal DurationinHours { get; set; }

        // experience date

        public DateTime ExperienceDate { get; set; }


        // experience location
        public string ExperienceLocation { get; set; }


        // the moods felt during this experience
        public List<string>? ExperienceMoods { get; set; }
    }
}

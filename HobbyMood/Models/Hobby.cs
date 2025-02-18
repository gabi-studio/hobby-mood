// using DataAnnotations necessary to indicate key attributes

using System.ComponentModel.DataAnnotations;

namespace HobbyMood.Models
{
    public class Hobby
    {
        // hobby id
        [Key]
        public int HobbyId { get; set; }


        // hobby name
        public required string HobbyName { get; set; }

        // link to experiences -- one hobby can be related to many experiences
        public virtual ICollection<Experience> Experiences { get; set; }

    }


    public class HobbyDto
    {
        // hobby id
        public int HobbyId { get; set; }

        // hobby name comes from the hobbies table
        public string? HobbyName { get; set; }

        // the number of experiences will come from all the experiences that have this hobby id in the experiences table
        public int NumberofExperiences { get; set; }

        // the number of hours spent will be summed from all the experiences that have this hobby id in the experiences table
        public decimal HoursSpent { get; set; }

        // the typical moods will be a list of moods that that are felt during experiences that have this hobby id in the experiences table that appear 3 or more times
        public List<string>? TypicalMoods { get; set; }
    }
}

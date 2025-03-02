// using DataAnnotations necessary to indicate key attributes

using System.ComponentModel.DataAnnotations;

namespace HobbyMood.Models
{
    public class Mood
    {

        // mood id
        [Key] 
        public int MoodId { get; set; }

        // mood name
        public required string MoodName { get; set; }
    }


    // mood dto

    public class MoodDto
    {
        // mood id
        public int MoodId { get; set; }

        // mood name
        public string? MoodName { get; set; }

        //// top 3 hobbies that are associated with this mood
        //public List<string>? TopHobbies { get; set; }

        // count of experiences that have this mood recorded
        public int MoodExperienceCount { get; set; }
    }
}

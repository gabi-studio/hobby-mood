// using DataAnnotations necessary to indicate key attributes
using System.ComponentModel.DataAnnotations;

namespace HobbyMood.Models
{
    public class ExperienceMood
    {
        // an individual occurence of a mood felt during an experience
        [Key] public int ExperienceMoodId { get; set; }


        // an experience where a mood is felt
        public required virtual Experience? Experience { get; set; }
        public int ExperienceId { get; set; }


        // a mood felt during an experience
        public required virtual Mood? Mood { get; set; } 
        public int MoodId { get; set; }


        // intensity of this mood before the experience
        public int MoodIntensityBefore { get; set; }

        // intensity of this mood after the experience
        public int MoodIntensityAfter { get; set; }
    }

    public class ExperienceMoodDto


    {
        // experiencemood id
        public int ExperienceMoodId { get; set; }

        // experience id
        public int ExperienceId { get; set; }


        // mood id
        public int MoodId { get; set; }


        // experience name comes from the experiences table
        public string ExperienceName { get; set; }


        // mood name comes from the moods table
        public string MoodName { get; set; }

        // date of experience and mood log comes from the experiences table
        public DateTime ExperienceDate { get; set; }

        // mood intensity before the experience
        public int MoodIntensityBefore { get; set; }


        // mood intensity after the experience
        public int MoodIntensityAfter { get; set; }
    }
}

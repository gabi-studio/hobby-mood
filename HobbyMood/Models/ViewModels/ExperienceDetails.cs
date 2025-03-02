using System.Collections.Generic;

namespace HobbyMood.Models.ViewModels
{
    public class ExperienceDetails
    {
        public ExperienceDto Experience { get; set; }
        public IEnumerable<HobbyDto> HobbyOptions { get; set; } = new List<HobbyDto>(); 
        public IEnumerable<MoodDto> MoodOptions { get; set; } = new List<MoodDto>();  
        public IEnumerable<ExperienceMoodDto> ExperienceMoods { get; set; } = new List<ExperienceMoodDto>();

        
    }
}

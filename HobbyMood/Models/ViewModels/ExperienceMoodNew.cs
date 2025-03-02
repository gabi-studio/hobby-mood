using HobbyMood.Models;

namespace HobbyMood.Models.ViewModels
{
    public class ExperienceMoodNew
    {
        public IEnumerable<ExperienceDto> AllExperiences { get; set; } = new List<ExperienceDto>();
        public IEnumerable<MoodDto> AllMoods { get; set; } = new List<MoodDto>();
    }
}

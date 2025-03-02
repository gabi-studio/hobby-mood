namespace HobbyMood.Models.ViewModels
{
    public class HobbyDetails
    {
        public HobbyDto Hobby { get; set; } = null!;
        public IEnumerable<ExperienceDto> HobbyExperiences { get; set; } = new List<ExperienceDto>();
        public IEnumerable<MoodDto> AllMoods { get; set; } = new List<MoodDto>();
        public IEnumerable<ExperienceMoodDto> ExperienceMoods { get; set; } = new List<ExperienceMoodDto>();
    }
}

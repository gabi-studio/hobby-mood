namespace HobbyMood.Models.ViewModels
{
    public class MoodDetails
    {
        public MoodDto Mood { get; set; }
        public IEnumerable<ExperienceDto> RelatedExperiences { get; set; } = new List<ExperienceDto>();
    }
}


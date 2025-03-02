namespace HobbyMood.Models.ViewModels
{
    public class ExperienceEdit
    {
        public ExperienceDto Experience { get; set; }
        public IEnumerable<HobbyDto> HobbyOptions { get; set; }
        public List<MoodDto> MoodOptions { get; set; } = new List<MoodDto>();
        public List<ExperienceMoodDto> ExperienceMoods { get; set; } = new List<ExperienceMoodDto>();

    }
}

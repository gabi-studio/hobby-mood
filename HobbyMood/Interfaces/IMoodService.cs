using HobbyMood.Models;

namespace HobbyMood.Interfaces
{
    public interface IMoodService
    {
        // list all moods
        Task<IEnumerable<MoodDto>> ListMoods();

        // find a specific mood
        Task<MoodDto?> FindMood(int id);

        // list experiences for a specific mood
        Task<IEnumerable<ExperienceDto>> ListExperiencesForMood(int moodId);

        // list moods for a specific experience
        Task<IEnumerable<MoodDto>> ListMoodsForExperience(int experienceId);


        // add a mood
        Task<ServiceResponse> AddMood(MoodDto moodDto);

        // update a mood
        Task<ServiceResponse> UpdateMood(int id, MoodDto moodDto);

        // delete a mood
        Task<ServiceResponse> DeleteMood(int id);
    }
}

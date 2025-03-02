using HobbyMood.Models;

namespace HobbyMood.Interfaces
{
    public interface IExperienceMoodService
    {
        // list all experience moods
        Task<IEnumerable<ExperienceMoodDto>> ListExperienceMoods();

        // find a specific experience mood
        Task<ExperienceMoodDto?> FindExperienceMood(int id);

        // add a new experience mood
        Task<ServiceResponse> AddExperienceMood(ExperienceMoodDto experienceMoodDto);

        // update an existing experience mood
        Task<ServiceResponse> UpdateExperienceMood(int id, ExperienceMoodDto experienceMoodDto);

        // delete an experience mood
        Task<ServiceResponse> DeleteExperienceMood(int id);

        // list moods for a specific experience
        Task<IEnumerable<ExperienceMoodDto>> ListExperienceMoodsForExperience(int experienceId);

        // link an experience to a mood
        Task<ServiceResponse> LinkExperienceToMood(int experienceId, int moodId, int? moodIntensityBefore, int? moodIntensityAfter);

        // unlink an experience from a mood
        Task<ServiceResponse> UnlinkExperienceFromMood(int experienceMoodId);


    }
}

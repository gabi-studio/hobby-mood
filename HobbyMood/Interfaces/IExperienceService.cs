using HobbyMood.Models;

namespace HobbyMood.Interfaces
{
    public interface IExperienceService
    {
        // list all experiences (only ID and Name)
        Task<IEnumerable<ExperienceDto>> ListExperiences();

        // find a specific experience (only ID and Name)
        Task<ExperienceDto?> FindExperience(int id);

        // list experiences related to a specific hobby
        Task<IEnumerable<ExperienceDto>> ListExperiencesForHobby(int hobbyId);

        //list experiences related to a specific mood
        Task<IEnumerable<ExperienceDto>> ListExperiencesForMood(int moodId);


        // add an experience
        Task<ServiceResponse> AddExperience(ExperienceDto experienceDto);

        // update an experience
        Task<ServiceResponse> UpdateExperience(int id, ExperienceDto experienceDto);

        // delete an experience
        Task<ServiceResponse> DeleteExperience(int id);

        // link an experience to a hobby
        Task<ServiceResponse> LinkExperienceToHobby(int experienceId, int hobbyId);

        // unlink an experience from a hobby
        Task<ServiceResponse> UnlinkExperienceFromHobby(int experienceId);

        // update experience image
        Task<ServiceResponse> UpdateExperienceImage(int id, IFormFile experienceImage);

    }
}

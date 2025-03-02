using HobbyMood.Models;

namespace HobbyMood.Interfaces
{
    public interface IHobbyService
    {
        // list all hobbies
        Task<IEnumerable<HobbyDto>> ListHobbies();

        // find a specific hobby by ID
        Task<HobbyDto?> FindHobby(int id);

        // list experiences related to a specific hobby
        Task<IEnumerable<ExperienceDto>> ListExperiencesForHobby(int hobbyId);

        // add a new hobby
        Task<ServiceResponse> AddHobby(HobbyDto hobbyDto);

        // update an existing hobby
        Task<ServiceResponse> UpdateHobby(int id, HobbyDto hobbyDto);

        // delete a hobby by ID
        Task<ServiceResponse> DeleteHobby(int id);

        // link a hobby to an experience
        Task<ServiceResponse> LinkHobbyToExperience(int hobbyId, int experienceId);

        // unlink a hobby from an experience
        Task<ServiceResponse> UnlinkHobbyFromExperience(int hobbyId, int experienceId);
    }
}

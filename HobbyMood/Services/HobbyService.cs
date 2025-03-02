using HobbyMood.Interfaces;
using HobbyMood.Models;
using HobbyMood.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HobbyMood.Services
{
    public class HobbyService : IHobbyService
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of database context
        public HobbyService(ApplicationDbContext context)
        {
            _context = context;
        }


        // List all hobbies
        public async Task<IEnumerable<HobbyDto>> ListHobbies()
        {
            // all hobbies and include related experiences
            List<Hobby> hobbies = await _context.Hobbies
                .Include(h => h.Experiences)
                .ToListAsync();

            // empty list of hobby DTOs
            List<HobbyDto> hobbyDtos = new();

            // foreach hobby, create a DTO and add to list
            foreach (Hobby hobby in hobbies)
            {
                hobbyDtos.Add(new HobbyDto()
                {
                    HobbyId = hobby.HobbyId,
                    HobbyName = hobby.HobbyName,
                    NumberofExperiences = hobby.Experiences.Count,
                    HoursSpent = hobby.Experiences.Sum(e => e.DurationinHours)
                });
            }
            return hobbyDtos;
        }

        // Find a specific hobby by ID
        public async Task<HobbyDto?> FindHobby(int id)
        {
            // Find the hobby including its experiences
            var hobby = await _context.Hobbies
                .Include(h => h.Experiences)
                .FirstOrDefaultAsync(h => h.HobbyId == id);

            // if not hobby found, return null
            if (!await HobbyExists(id))
            {
                return null;
            }

            // return  a new isntance of Hobby DTO
            return new HobbyDto()
            {
                HobbyId = hobby.HobbyId,
                HobbyName = hobby.HobbyName,
                NumberofExperiences = hobby.Experiences.Count,
                HoursSpent = hobby.Experiences.Sum(e => e.DurationinHours)
            };
        }



        // List experiences related to a specific hobby
        public async Task<IEnumerable<ExperienceDto>> ListExperiencesForHobby(int hobbyId)
        {

            // join experiences with hobbies and filter by hobby ID
            List<Experience> experiences = await _context.Experiences
                .Include(e => e.Hobby)
                .Where(e => e.HobbyId == hobbyId)
                .ToListAsync();

            // empty list of experience DTOs
            List<ExperienceDto> experienceDtos = new();
            // foreach experience, create a new DTO and add to list
            foreach (Experience e in experiences)
            {
                experienceDtos.Add(new ExperienceDto()
                {
                    ExperienceId = e.ExperienceId,
                    ExperienceName = e.ExperienceName,
                    HobbyId = e.HobbyId,
                    HobbyName = e.Hobby?.HobbyName,
                    ExperienceCost = e.ExperienceCost,
                    DurationinHours = e.DurationinHours,
                    ExperienceDate = e.ExperienceDate,
                    ExperienceLocation = e.ExperienceLocation
                });
            }

            return experienceDtos;
        }

        // Add a new hobby
        public async Task<ServiceResponse> AddHobby(HobbyDto hobbyDto)
        {
            ServiceResponse response = new();

            // Validate input to ensure hobby name is not empty
            if (string.IsNullOrWhiteSpace(hobbyDto.HobbyName))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Hobby name cannot be empty.");
                return response;
            }

            try
            {
                // Create new Hobby object
                Hobby hobby = new Hobby()
                {
                    HobbyName = hobbyDto.HobbyName
                };

                _context.Hobbies.Add(hobby);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = hobby.HobbyId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while adding the hobby.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }



        // Update an existing hobby
        public async Task<ServiceResponse> UpdateHobby(int id, HobbyDto hobbyDto)
        {
            ServiceResponse response = new();

            // Validate hobby ID
            if (id != hobbyDto.HobbyId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Hobby ID mismatch.");
                return response;
            }

            var existingHobby = await _context.Hobbies.FindAsync(id);
            if (existingHobby == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Hobby not found.");
                return response;
            }

            // Update hobby properties
            existingHobby.HobbyName = hobbyDto.HobbyName;
            _context.Entry(existingHobby).State = EntityState.Modified;

            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HobbyExists(id))
                {
                    response.Status = ServiceResponse.ServiceStatus.NotFound;
                    response.Messages.Add("Hobby not found.");
                    return response;
                }
                else
                {
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    response.Messages.Add("An error occurred updating the record.");
                    return response;
                }
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }



        // Delete a hobby
        public async Task<ServiceResponse> DeleteHobby(int id)
        {
            ServiceResponse response = new();

            // Find the hobby by ID
            var hobby = await _context.Hobbies.FindAsync(id);
            if (!await HobbyExists(id))
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Hobby not found.");
                return response;
            }

            try
            {
                _context.Hobbies.Remove(hobby);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting the hobby.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }


        // Link a hobby to an experience
        public async Task<ServiceResponse> LinkHobbyToExperience(int hobbyId, int experienceId)
        {
            ServiceResponse response = new();

            Hobby? hobby = await _context.Hobbies
                .Include(h => h.Experiences)
                .FirstOrDefaultAsync(h => h.HobbyId == hobbyId);
            Experience? experience = await _context.Experiences.FindAsync(experienceId);

            // valodate that hobby and experience exist
            if (hobby == null || experience == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;

                if (hobby == null) response.Messages.Add("Hobby was not found.");
                if (experience == null) response.Messages.Add("Experience was not found.");
                return response;
            }

            try
            {
                hobby.Experiences.Add(experience);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error linking the hobby to the experience.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Created;
            return response;
        }


        // Unlink a hobby from an experience
        public async Task<ServiceResponse> UnlinkHobbyFromExperience(int hobbyId, int experienceId)
        {
            ServiceResponse response = new();

            Hobby? hobby = await _context.Hobbies
                .Include(h => h.Experiences)
                .FirstOrDefaultAsync(h => h.HobbyId == hobbyId);
            Experience? experience = await _context.Experiences.FindAsync(experienceId);

            if (hobby == null || experience == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                if (hobby == null) response.Messages.Add("Hobby was not found.");
                if (experience == null) response.Messages.Add("Experience was not found.");
                return response;
            }

            try
            {
                hobby.Experiences.Remove(experience);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error unlinking the hobby from the experience.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        // to use to check if a hobby exists
     
        private async Task<bool> HobbyExists(int id)
        {
            return await _context.Hobbies.AnyAsync(h => h.HobbyId == id);
        }
    }
}

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
    public class MoodService : IMoodService
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of database context
        public MoodService(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all moods
        public async Task<IEnumerable<MoodDto>> ListMoods()
        {
            // Retrieve all moods from the database
            List<Mood> moods = await _context.Moods.ToListAsync();

            
            List<MoodDto> moodDtos = new();

            // foreach mood, create a DTO and add to list
            foreach (Mood mood in moods)
            {
                moodDtos.Add(new MoodDto()
                {
                    MoodId = mood.MoodId,
                    MoodName = mood.MoodName,
                    MoodExperienceCount = _context.ExperienceMoods.Count(em => em.MoodId == mood.MoodId)
                });
            }
            return moodDtos;
        }

        // Find a specific mood by ID
        public async Task<MoodDto?> FindMood(int id)
        {
            // Find the mood in the database
            var mood = await _context.Moods
                .FirstOrDefaultAsync(m => m.MoodId == id);

            // If no mood is found, return null
            if (!await MoodExists(id))
            {
                return null;
            }

            // create and return an instance of MoodDto
            return new MoodDto()
            {
                MoodId = mood.MoodId,
                MoodName = mood.MoodName,
                MoodExperienceCount = _context.ExperienceMoods.Count(em => em.MoodId == mood.MoodId)
            };
        }


        // List experiences related to a specific mood
        public async Task<IEnumerable<ExperienceDto>> ListExperiencesForMood(int moodId)
        {
            // join ExperienceMood and Experience tables to retrieve experiences linked to this mood
            List<Experience> experiences = await _context.ExperienceMoods
                .Where(em => em.MoodId == moodId)
                .Include(em => em.Experience)
                .Select(em => em.Experience)
                .Distinct()
                .ToListAsync();

            
            List<ExperienceDto> experienceDtos = new();

            // for each experience, create a DTO and add to list
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

        // List moods associated with a specific experience
        public async Task<IEnumerable<MoodDto>> ListMoodsForExperience(int experienceId)
        {
            // Retrieve moods linked to this experience
            List<Mood> moods = await _context.ExperienceMoods
                .Where(em => em.ExperienceId == experienceId)
                .Select(em => em.Mood)
                .ToListAsync();
            
            List<MoodDto> moodDtos = new();

            // for each mood, create a DTO and add to list
            foreach (Mood mood in moods)
            {
                moodDtos.Add(new MoodDto()
                {
                    MoodId = mood.MoodId,
                    MoodName = mood.MoodName
                });
            }
            return moodDtos;
        }

        // add a new mood
        public async Task<ServiceResponse> AddMood(MoodDto moodDto)
        {
            ServiceResponse response = new();

            // validate that mood name is not empty
            if (string.IsNullOrWhiteSpace(moodDto.MoodName))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Mood name cannot be empty.");
                return response;
            }

            try
            {
                // Create a new Mood object
                Mood mood = new()
                {
                    MoodName = moodDto.MoodName
                };

                _context.Moods.Add(mood);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = mood.MoodId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding the mood.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Update an existing mood
        public async Task<ServiceResponse> UpdateMood(int id, MoodDto moodDto)
        {
            ServiceResponse response = new();

            // Validate mood ID
            if (id != moodDto.MoodId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Mood ID mismatch.");
                return response;
            }

            var existingMood = await _context.Moods.FindAsync(id);

            if (existingMood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Mood not found.");
                return response;
            }

            try
            {
                // Update mood properties
                existingMood.MoodName = moodDto.MoodName;
                _context.Entry(existingMood).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MoodExists(id))
                {
                    response.Status = ServiceResponse.ServiceStatus.NotFound;
                    response.Messages.Add("Mood not found.");
                }
                else
                {
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    response.Messages.Add("Concurrency error updating the mood.");
                }
            }

            return response;
        }

        // Delete a mood
        public async Task<ServiceResponse> DeleteMood(int id)
        {
            ServiceResponse response = new();

            var mood = await _context.Moods.FindAsync(id);

            if (!await MoodExists(id))
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Mood not found.");
                return response;
            }

            try
            {
                _context.Moods.Remove(mood);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting the mood.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Checks if a mood exists
        private async Task<bool> MoodExists(int id)
        {
            return await _context.Moods.AnyAsync(m => m.MoodId == id);
        }
    }
}

using HobbyMood.Interfaces;
using HobbyMood.Models;
using HobbyMood.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace HobbyMood.Services
{
    public class ExperienceMoodService : IExperienceMoodService
    {
        private readonly ApplicationDbContext _context;

        // dependency injection of database context
        public ExperienceMoodService(ApplicationDbContext context)
        {
            _context = context;
        }


        // List all ExperienceMoods
        public async Task<IEnumerable<ExperienceMoodDto>> ListExperienceMoods()
        {
            // Retrieve all ExperienceMoods including related Experiences and Moods
            List<ExperienceMood> experienceMoods = await _context.ExperienceMoods
                .Include(em => em.Experience)
                .Include(em => em.Mood)
                .ToListAsync();

            // Convert each ExperienceMood entity to DTO format
            List<ExperienceMoodDto> experienceMoodDtos = new();
            foreach (ExperienceMood em in experienceMoods)
            {
                experienceMoodDtos.Add(new ExperienceMoodDto()
                {
                    ExperienceMoodId = em.ExperienceMoodId,
                    ExperienceId = em.ExperienceId,
                    MoodId = em.MoodId,
                    ExperienceName = em.Experience.ExperienceName,
                    MoodName = em.Mood.MoodName,
                    ExperienceDate = em.Experience.ExperienceDate,
                    MoodIntensityBefore = em.MoodIntensityBefore,
                    MoodIntensityAfter = em.MoodIntensityAfter
                });
            }
            return experienceMoodDtos;
        }

        // find a specific ExperienceMood by ID
 
        public async Task<ExperienceMoodDto?> FindExperienceMood(int id)
        {
            //retrieve ExperienceMood including related Experience and Mood
            var experienceMood = await _context.ExperienceMoods
                .Include(em => em.Experience)
                .Include(em => em.Mood)
                .FirstOrDefaultAsync(em => em.ExperienceMoodId == id);

            // if not found, return null
            if (experienceMood == null)
            {
                return null;
            }

            // create and return DTO instance
            return new ExperienceMoodDto()
            {
                ExperienceMoodId = experienceMood.ExperienceMoodId,
                ExperienceId = experienceMood.ExperienceId,
                MoodId = experienceMood.MoodId,
                ExperienceName = experienceMood.Experience.ExperienceName,
                MoodName = experienceMood.Mood.MoodName,
                ExperienceDate = experienceMood.Experience.ExperienceDate,
                MoodIntensityBefore = experienceMood.MoodIntensityBefore,
                MoodIntensityAfter = experienceMood.MoodIntensityAfter
            };
        }

        // add a new ExperienceMood
        public async Task<ServiceResponse> AddExperienceMood(ExperienceMoodDto experienceMoodDto)
        {
            ServiceResponse response = new();

            // Validate Experience and Mood exist
            var experience = await _context.Experiences.FindAsync(experienceMoodDto.ExperienceId);
            var mood = await _context.Moods.FindAsync(experienceMoodDto.MoodId);

            if (experience == null || mood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Invalid ExperienceId or MoodId.");
                return response;
            }

            try
            {
                // Create ExperienceMood entity
                ExperienceMood experienceMood = new()
                {
                    ExperienceId = experience.ExperienceId,
                    MoodId = mood.MoodId,
                    Experience = experience,
                    Mood = mood,
                    MoodIntensityBefore = experienceMoodDto.MoodIntensityBefore,
                    MoodIntensityAfter = experienceMoodDto.MoodIntensityAfter
                };

                _context.ExperienceMoods.Add(experienceMood);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = experienceMood.ExperienceMoodId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("There was an error adding the ExperienceMood.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Update an existing ExperienceMood
        public async Task<ServiceResponse> UpdateExperienceMood(int id, ExperienceMoodDto experienceMoodDto)
        {
            ServiceResponse response = new();

            // Validate ExperienceMood ID
            if (id != experienceMoodDto.ExperienceMoodId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("ExperienceMood ID mismatch.");
                return response;
            }

            var existingExperienceMood = await _context.ExperienceMoods.FindAsync(id);
            if (existingExperienceMood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("ExperienceMood not found.");
                return response;
            }

            // Validate Experience and Mood exist
            var experience = await _context.Experiences.FindAsync(experienceMoodDto.ExperienceId);
            var mood = await _context.Moods.FindAsync(experienceMoodDto.MoodId);

            if (experience == null || mood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Invalid ExperienceId or MoodId.");
                return response;
            }

            try
            {
                // Update ExperienceMood properties
                existingExperienceMood.ExperienceId = experience.ExperienceId;
                existingExperienceMood.MoodId = mood.MoodId;
                existingExperienceMood.Experience = experience;
                existingExperienceMood.Mood = mood;
                existingExperienceMood.MoodIntensityBefore = experienceMoodDto.MoodIntensityBefore;
                existingExperienceMood.MoodIntensityAfter = experienceMoodDto.MoodIntensityAfter;

                _context.Entry(existingExperienceMood).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExperienceMoodExists(id))
                {
                    response.Status = ServiceResponse.ServiceStatus.NotFound;
                    response.Messages.Add("ExperienceMood not found.");
                }
                else
                {
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    response.Messages.Add("Concurrency error updating the ExperienceMood.");
                }
            }

            return response;
        }

        // Delete an ExperienceMood
        public async Task<ServiceResponse> DeleteExperienceMood(int id)
        {
            ServiceResponse response = new();

            var experienceMood = await _context.ExperienceMoods.FindAsync(id);
            if (experienceMood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("ExperienceMood not found.");
                return response;
            }

            try
            {
                _context.ExperienceMoods.Remove(experienceMood);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting the ExperienceMood.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // check if ExperienceMood exists
        private async Task<bool> ExperienceMoodExists(int id)
        {
            return await _context.ExperienceMoods.AnyAsync(em => em.ExperienceMoodId == id);
        }

        // List ExperienceMoods for a specific Experience
        public async Task<IEnumerable<ExperienceMoodDto>> ListExperienceMoodsForExperience(int experienceId)
        {
            List<ExperienceMood> experienceMoods = await _context.ExperienceMoods
                .Where(em => em.ExperienceId == experienceId)
                .Include(em => em.Mood)
                .ToListAsync();

            List<ExperienceMoodDto> experienceMoodDtos = new();
            foreach (ExperienceMood em in experienceMoods)
            {
                experienceMoodDtos.Add(new ExperienceMoodDto()
                {
                    ExperienceMoodId = em.ExperienceMoodId,
                    ExperienceId = em.ExperienceId,
                    MoodId = em.MoodId,
                    ExperienceName = em.Experience.ExperienceName,
                    MoodName = em.Mood.MoodName,
                    ExperienceDate = em.Experience.ExperienceDate,
                    MoodIntensityBefore = em.MoodIntensityBefore,
                    MoodIntensityAfter = em.MoodIntensityAfter
                });
            }
            return experienceMoodDtos;
        }


       
        public async Task<ServiceResponse> LinkExperienceToMood(int experienceId, int moodId, int? moodIntensityBefore, int? moodIntensityAfter)
        {
            ServiceResponse response = new();

            // check if the experience and mood exist
            var experience = await _context.Experiences.FindAsync(experienceId);
            var mood = await _context.Moods.FindAsync(moodId);

            if (experience == null || mood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                if (experience == null) response.Messages.Add("Experience not found.");
                if (mood == null) response.Messages.Add("Mood not found.");
                return response;
            }

            try
            {
               
                ExperienceMood experienceMood = new()
                {
                    ExperienceId = experienceId,
                    MoodId = moodId,
                    Experience = experience,  
                    Mood = mood,  
                    MoodIntensityBefore = moodIntensityBefore ?? 0,  
                    MoodIntensityAfter = moodIntensityAfter ?? 0  
                };

                // add to database
                _context.ExperienceMoods.Add(experienceMood);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error linking experience to mood.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // unlink experience from mood
        public async Task<ServiceResponse> UnlinkExperienceFromMood(int experienceMoodId)
        {
            ServiceResponse response = new();

            var experienceMood = await _context.ExperienceMoods
                .FirstOrDefaultAsync(em => em.ExperienceMoodId == experienceMoodId);

            if (experienceMood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Link between experience and mood not found.");
                return response;
            }

            try
            {
                _context.ExperienceMoods.Remove(experienceMood);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error unlinking experience from mood.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }


    }
}

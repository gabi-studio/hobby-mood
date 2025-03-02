using HobbyMood.Interfaces;
using HobbyMood.Models;
using HobbyMood.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace HobbyMood.Services
{
    public class ExperienceService : IExperienceService
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of database context
        public ExperienceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // list all experiences 
        public async Task<IEnumerable<ExperienceDto>> ListExperiences()
        {
            // all experiences
            List<Experience> experiences = await _context.Experiences
                .Include(e => e.Hobby)
                .ToListAsync();

            // empty list of data transfer object ExperienceDto
            List<ExperienceDto> experienceDtos = new List<ExperienceDto>();

            // foreach Experience record in database
            foreach (Experience experience in experiences)
            {
                // create new instance of ExperienceDto, add to list
                experienceDtos.Add(new ExperienceDto()
                {
                    
                    ExperienceId = experience.ExperienceId,
                    ExperienceName = experience.ExperienceName,
                    HobbyId = experience.HobbyId,
                    HobbyName = experience.Hobby != null ? experience.Hobby.HobbyName : "No Hobby Associated",
                    ExperienceDate = experience.ExperienceDate,
                    ExperienceLocation = experience.ExperienceLocation,
                    DurationinHours = experience.DurationinHours,
                    ExperienceCost = experience.ExperienceCost,
                    //ExperienceMoods = experience.ExperienceMoods
                    //    .Where(em => em.ExperienceId == experience.ExperienceId)
                    //    .Select(em => em.Mood.MoodName)
                    //    .ToList() ?? new List<string>()



                });
            }
            // return ExperienceDtos
            return experienceDtos;
        }

        // find a specific experience 
        public async Task<ExperienceDto?> FindExperience(int id)
        {
            var experience = await _context.Experiences
                .Include(e => e.Hobby)
                .Include(e => e.ExperienceMoods)
                    .ThenInclude(em => em.Mood)
                .FirstOrDefaultAsync(e => e.ExperienceId == id);

            if (experience == null)
            {
                return null;
            }

            return new ExperienceDto()
            {
                ExperienceId = experience.ExperienceId,
                ExperienceName = experience.ExperienceName,
                HobbyId = experience.HobbyId,
                HobbyName = experience.Hobby?.HobbyName,
                ExperienceCost = experience.ExperienceCost,
                DurationinHours = experience.DurationinHours,
                ExperienceDate = experience.ExperienceDate,
                ExperienceLocation = experience.ExperienceLocation
                //ExperienceMoods = experience.ExperienceMoods
                //    .Select(em => new ExperienceMoodDto()
                //    {
                //        ExperienceMoodId = em.ExperienceMoodId,
                //        MoodId = em.MoodId,
                //        MoodName = em.Mood.MoodName,
                //        MoodIntensityBefore = em.MoodIntensityBefore,
                //        MoodIntensityAfter = em.MoodIntensityAfter
                //    }).ToList()
            };
        }


        // list experiences related to a specific hobby
        public async Task<IEnumerable<ExperienceDto>> ListExperiencesForHobby(int hobbyId)
        {
            // where experiences have HobbyId = {hobbyId}
            List<Experience> experiences = await _context.Experiences
                .Where(e => e.HobbyId == hobbyId)
                .ToListAsync();

            // empty list of data transfer object ExperienceDto
            List<ExperienceDto> experienceDtos = new List<ExperienceDto>();

            //foreach Experience record in database
            foreach (Experience experience in experiences)
            {
                // create new instance of ExperienceDto, add to list
                experienceDtos.Add(new ExperienceDto()
                {
                    ExperienceId = experience.ExperienceId,
                    ExperienceName = experience.ExperienceName,
                    HobbyId = experience.HobbyId,
                    HobbyName = experience.Hobby?.HobbyName,
                    ExperienceCost = experience.ExperienceCost,
                    DurationinHours = experience.DurationinHours,
                    ExperienceDate = experience.ExperienceDate,
                    ExperienceLocation = experience.ExperienceLocation
                    //ExperienceMoods = _context.ExperienceMoods
                    //    .Where(em => em.ExperienceId == experience.ExperienceId)
                    //    .Select(em => em.Mood.MoodName)
                    //    .ToList()
                });
            }
            // return ExperienceDtos
            return experienceDtos;
        }

        // list experiences related to a specific mood
        public async Task<IEnumerable<ExperienceDto>> ListExperiencesForMood(int moodId)
        {
            List<ExperienceMood> experienceMoods = await _context.ExperienceMoods
                .Include(em => em.Experience)
                .Where(em => em.MoodId == moodId)
                .ToListAsync();

            return experienceMoods.Select(em => new ExperienceDto
            {
                ExperienceId = em.Experience.ExperienceId,
                ExperienceName = em.Experience.ExperienceName,
                HobbyId = em.Experience.HobbyId,
                HobbyName = em.Experience.Hobby?.HobbyName,
                ExperienceCost = em.Experience.ExperienceCost,
                DurationinHours = em.Experience.DurationinHours,
                ExperienceDate = em.Experience.ExperienceDate,
                ExperienceLocation = em.Experience.ExperienceLocation
            }).ToList();
        }


        // add an experience
        public async Task<ServiceResponse> AddExperience(ExperienceDto experienceDto)
        {
            ServiceResponse serviceResponse = new();

            // Ensure experience links to a valid Hobby
            var hobby = await _context.Hobbies.FindAsync(experienceDto.HobbyId);
            if (hobby == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Hobby not found.");
                return serviceResponse;
            }

            // Create an instance of Experience
            Experience experience = new Experience()
            {
                ExperienceName = experienceDto.ExperienceName,
                HobbyId = hobby.HobbyId,
                ExperienceCost = experienceDto.ExperienceCost,
                DurationinHours = experienceDto.DurationinHours,
                ExperienceDate = experienceDto.ExperienceDate,
                ExperienceLocation = experienceDto.ExperienceLocation
            };

            try
            {
                _context.Experiences.Add(experience);
                await _context.SaveChangesAsync(); 
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the experience.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = experience.ExperienceId;
            return serviceResponse;
        }



        // update an experience
        public async Task<ServiceResponse> UpdateExperience(int id, ExperienceDto experienceDto)
        {
            ServiceResponse serviceResponse = new();

            if (id != experienceDto.ExperienceId)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Experience ID mismatch.");
                return serviceResponse;
            }

            var existingExperience = await _context.Experiences
                .Include(e => e.ExperienceMoods)
                .FirstOrDefaultAsync(e => e.ExperienceId == id);

            if (existingExperience == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Experience not found.");
                return serviceResponse;
            }

            _context.Entry(existingExperience).State = EntityState.Detached;

            var hobby = await _context.Hobbies.FindAsync(experienceDto.HobbyId);
            if (hobby == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Hobby not found.");
                return serviceResponse;
            }

            // update Experience fields
            existingExperience.ExperienceName = experienceDto.ExperienceName;
            existingExperience.HobbyId = hobby.HobbyId;
            existingExperience.ExperienceCost = experienceDto.ExperienceCost;
            existingExperience.DurationinHours = experienceDto.DurationinHours;
            existingExperience.ExperienceDate = experienceDto.ExperienceDate;
            existingExperience.ExperienceLocation = experienceDto.ExperienceLocation;

            // mood intensity update
            foreach (var moodDto in experienceDto.ExperienceMoods)
            {
                var existingMood = _context.ExperienceMoods
                    .FirstOrDefault(em => em.ExperienceMoodId == moodDto.ExperienceMoodId);

                if (existingMood != null)
                {
                    existingMood.MoodIntensityBefore = moodDto.MoodIntensityBefore;
                    existingMood.MoodIntensityAfter = moodDto.MoodIntensityAfter;

                    _context.Entry(existingMood).State = EntityState.Modified;
                }
            }

            try
            {
                _context.Experiences.Attach(existingExperience);
                _context.Entry(existingExperience).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the record.");
            }

            return serviceResponse;
        }




        // delete an experience
        public async Task<ServiceResponse> DeleteExperience(int id)
        {
            ServiceResponse serviceResponse = new();

            // experience must exist in the first place
            var experience = await _context.Experiences.FindAsync(id);
            if (experience == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Experience cannot be deleted because it does not exist.");
                return serviceResponse;
            }

            try
            {
                // delete related ExperienceMoods
                var experienceMoods = _context.ExperienceMoods.Where(em => em.ExperienceId == id);
                _context.ExperienceMoods.RemoveRange(experienceMoods);

                _context.Experiences.Remove(experience);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error encountered while deleting the experience.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }

        // link an experience to a hobby
        public async Task<ServiceResponse> LinkExperienceToHobby(int experienceId, int hobbyId)
        {
            ServiceResponse serviceResponse = new();

            Experience? experience = await _context.Experiences.FindAsync(experienceId);
            Hobby? hobby = await _context.Hobbies.FindAsync(hobbyId);

            // data must link to a valid entity
            if (experience == null || hobby == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (experience == null) serviceResponse.Messages.Add("Experience was not found.");
                if (hobby == null) serviceResponse.Messages.Add("Hobby was not found.");
                return serviceResponse;
            }

            try
            {
                experience.HobbyId = hobby.HobbyId;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an issue linking the experience to the hobby.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            return serviceResponse;
        }

        // unlink an experience from a hobby
        public async Task<ServiceResponse> UnlinkExperienceFromHobby(int experienceId)
        {
            ServiceResponse serviceResponse = new();

            Experience? experience = await _context.Experiences.FindAsync(experienceId);

            // data must link to a valid entity
            if (experience == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Experience was not found.");
                return serviceResponse;
            }

            try
            {

                experience.HobbyId = 0;  
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an issue unlinking the experience from the hobby.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }

        // check if experience exists
        private async Task<bool> ExperienceExists(int id)
        {
            return await _context.Experiences.AnyAsync(e => e.ExperienceId == id);
        }



        // update an experience image
        // to do: fix the issue of image uploading but not visible on views
        public async Task<ServiceResponse> UpdateExperienceImage(int experienceId, IFormFile experienceImage)
        {
            ServiceResponse response = new();

            // Find the experience in the database
            var experience = await _context.Experiences.FindAsync(experienceId);
            if (experience == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"Experience {experienceId} not found");
                return response;
            }

            // Validate image presence and content
            if (experienceImage == null || experienceImage.Length == 0)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("No image provided or file is empty");
                return response;
            }

            // Define allowed file extensions
            List<string> allowedExtensions = new() { ".jpeg", ".jpg", ".png", ".gif" };
            string imageExtension = Path.GetExtension(experienceImage.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(imageExtension))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add($"{imageExtension} is not a valid file extension");
                return response;
            }

            // Remove old image if it exists
            if (!string.IsNullOrEmpty(experience.ExperienceImagePath))
            {
                string oldFilePath = Path.Combine("wwwroot/images/experiences/", experience.ExperienceImagePath);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            // Generate and save new file
            string fileName = $"{experienceId}{imageExtension}";
            string filePath = Path.Combine("wwwroot/images/experiences/", fileName);

            using (var stream = File.Create(filePath))
            {
                await experienceImage.CopyToAsync(stream);
            }

            // Confirm successful upload
            if (File.Exists(filePath))
            {
                experience.ExperienceImagePath = fileName;
                _context.Entry(experience).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    response.Messages.Add("An error occurred updating the record");
                    return response;
                }
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            response.Messages.Add("Image uploaded successfully");
            return response;
        }




    }
}

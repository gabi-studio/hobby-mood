using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HobbyMood.Data;
using HobbyMood.Models;

namespace HobbyMood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceMoodController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of database context
        public ExperienceMoodController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of all experience moods with related experience and mood data.
        /// </summary>
        /// <returns>200 OK with ExperienceMoodDto List</returns>
        /// <example>GET: api/ExperienceMood/List => { "experienceMoodId": 1, "experienceId": 3, "moodId": 5, "experienceName": "Hiked Skyline Trail", "moodName": "Energized", "experienceDate": "2022-07-05", "moodIntensityBefore": 3, "moodIntensityAfter": 8 }</example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ExperienceMoodDto>>> ListExperienceMoods()
        {
            List<ExperienceMood> experienceMoods = await _context.ExperienceMoods
                .Include(em => em.Experience)
                .Include(em => em.Mood)
                .ToListAsync();

            // empty list of dto ExperienceMoodDto
            List<ExperienceMoodDto> experienceMoodDtos = new List<ExperienceMoodDto>();

            foreach (var em in experienceMoods)
            {
                // creating an instance of ExperienceMoodDto
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

            return Ok(experienceMoodDtos);
        }


        /// <summary>
        /// Returns a specific experience mood by ID.
        /// </summary>
        /// <param name="id">ExperienceMood ID</param>
        /// <returns>200 OK with ExperienceMoodDto, or 404 Not Found</returns>
        /// <example>GET: api/ExperienceMood/Find/1 => { "experienceMoodId": 1, "experienceId": 3, "moodId": 5, "experienceName": "Hiked Skyline Trail", "moodName": "Energized", "experienceDate": "2022-07-05", "moodIntensityBefore": 3, "moodIntensityAfter": 8 }</example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ExperienceMoodDto>> FindExperienceMood(int id)
        {
            // get the first experience mood matching the {id} 
            var experienceMoodEntity = await _context.ExperienceMoods
                .Include(em => em.Experience)
                .Include(em => em.Mood)
                .Where(em => em.ExperienceMoodId == id)
                .FirstOrDefaultAsync();

            // if the experience mood can't be located, return 404 Not Found
            if (experienceMoodEntity == null)
            {
                return NotFound();
            }

            // creating an instance of ExperienceMoodDto
            ExperienceMoodDto experienceMoodDto = new ExperienceMoodDto()
            {
                ExperienceMoodId = experienceMoodEntity.ExperienceMoodId,
                ExperienceId = experienceMoodEntity.ExperienceId,
                MoodId = experienceMoodEntity.MoodId,
                ExperienceName = experienceMoodEntity.Experience.ExperienceName,
                MoodName = experienceMoodEntity.Mood.MoodName,
                ExperienceDate = experienceMoodEntity.Experience.ExperienceDate,
                MoodIntensityBefore = experienceMoodEntity.MoodIntensityBefore,
                MoodIntensityAfter = experienceMoodEntity.MoodIntensityAfter
            };

            return Ok(experienceMoodDto);
        }


        /// <summary>
        /// Adds a new experience mood.
        /// </summary>
        /// <param name="experienceMoodDto">The required information to add the experience mood (ExperienceId, MoodId, MoodIntensityBefore, MoodIntensityAfter)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/ExperienceMood/Find/{ExperienceMoodId}
        /// {ExperienceMoodDto}
        /// or
        /// 400 Bad Request
        /// </returns>
        [HttpPost("Add")]
        public async Task<ActionResult<ExperienceMood>> AddExperienceMood(ExperienceMoodDto experienceMoodDto)
        {
            // validate existence of experience and mood
            var experience = await _context.Experiences.FindAsync(experienceMoodDto.ExperienceId);
            var mood = await _context.Moods.FindAsync(experienceMoodDto.MoodId);

            if (experience == null || mood == null)
            {
                return NotFound("Invalid ExperienceId or MoodId.");
            }

            // creating a new instance of ExperienceMood
            ExperienceMood experienceMood = new ExperienceMood()
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

            return CreatedAtAction(nameof(FindExperienceMood), new { id = experienceMood.ExperienceMoodId }, experienceMoodDto);
        }


        /// <summary>
        /// Updates an existing experience mood.
        /// </summary>
        /// <param name="id">ExperienceMood ID</param>
        /// <param name="experienceMoodDto">The required information to update the experience mood (ExperienceMoodId, ExperienceId, MoodId, MoodIntensityBefore, MoodIntensityAfter)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateExperienceMood(int id, ExperienceMoodDto experienceMoodDto)
        {
            // id in url must match ExperienceMoodId in POST body
            if (id != experienceMoodDto.ExperienceMoodId)
            {
                return BadRequest();
            }

            // finding the existing experience mood that matches
            var existingExperienceMood = await _context.ExperienceMoods.FindAsync(id);

            // if there is no existing experience mood
            if (existingExperienceMood == null)
            {
                return NotFound();
            }

            // Validate existence of Experience and Mood
            var experience = await _context.Experiences.FindAsync(experienceMoodDto.ExperienceId);
            var mood = await _context.Moods.FindAsync(experienceMoodDto.MoodId);

            if (experience == null || mood == null)
            {
                return NotFound("Invalid ExperienceId or MoodId.");
            }

            // updating the experience mood fields
            existingExperienceMood.ExperienceId = experience.ExperienceId;
            existingExperienceMood.MoodId = mood.MoodId;
            existingExperienceMood.MoodIntensityBefore = experienceMoodDto.MoodIntensityBefore;
            existingExperienceMood.MoodIntensityAfter = experienceMoodDto.MoodIntensityAfter;

            _context.Entry(existingExperienceMood).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Deletes an experience mood by ID.
        /// </summary>
        /// <param name="id">ExperienceMood ID</param>
        /// <returns>204 No Content, or 404 Not Found</returns>
        /// <example>DELETE: api/ExperienceMood/Delete/1</example>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteExperienceMood(int id)
        {
            // checking if the experience mood exists
            var experienceMood = await _context.ExperienceMoods.FindAsync(id);

            if (experienceMood == null)
            {
                return NotFound();
            }

            _context.ExperienceMoods.Remove(experienceMood);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
    }
}

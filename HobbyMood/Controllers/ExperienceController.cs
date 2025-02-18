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
    public class ExperienceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of database context
        public ExperienceController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of all experiences with related hobby and moods.
        /// </summary>
        /// <returns>200 OK with ExperienceDto List</returns>
        /// <example>GET: api/Experience/List => { "experienceId": 1, "experienceName": "Hiked the Skyline Trail", "hobbyName": "Hiking", "hobbyId": 2, "experienceCost": 0, "durationinHours": 3.75, "experienceDate": "2022-07-05T00:00:00", "experienceLocation": "Skyline Trail", "experienceMoods": [ "Energized", "Inpired", "Reflective" ] }, etc... </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ExperienceDto>>> ListExperiences()
        {

            List<Experience> experiences = await _context.Experiences
                .Include(e => e.Hobby)
                .ToListAsync();

            // empty list of dto ExperienceDto
            List<ExperienceDto> experienceDtos = new List<ExperienceDto>();

            foreach (var e in experiences)
            {
                //creating an instance of ExperienceDto
                experienceDtos.Add(new ExperienceDto()
                {
                    ExperienceId = e.ExperienceId,  
                    ExperienceName = e.ExperienceName,
                    HobbyId = e.HobbyId,
                    HobbyName = e.Hobby.HobbyName,
                    ExperienceCost = e.ExperienceCost,
                    DurationinHours = e.DurationinHours,
                    ExperienceDate = e.ExperienceDate,
                    ExperienceLocation = e.ExperienceLocation,

                    // getting the moods related to this experience as a list
                    ExperienceMoods = _context.ExperienceMoods
                        .Where(em => em.ExperienceId == e.ExperienceId)
                        .Select(em => em.Mood.MoodName)
                        .Distinct()
                        .ToList()
                });
            }

            return Ok(experienceDtos);
        }


        /// <summary>
        /// Returns a specific experience by ID.
        /// </summary>
        /// <param name="id">Experience ID</param>
        /// <returns>200 OK with ExperienceDto, or 404 Not Found</returns>
        /// <example>GET: api/Experience/Find/2 => { "experienceId": 2, "experienceName": "Did Lisboa Puzzle", "hobbyName": "Puzzles", "hobbyId": 9, "experienceCost": 25, "durationinHours": 6.5, "experienceDate": "2022-07-10T00:00:00", "experienceLocation": "Home", "experienceMoods": [ "Relaxed" ] }</example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ExperienceDto>> FindExperience(int id)
        {
            // get the first experience matching the {id} 
            var experienceEntity = await _context.Experiences
                .Include(e => e.Hobby)
                .Where(e => e.ExperienceId == id)
                .FirstOrDefaultAsync();
            

            // if thie experience can't be located, return 404 Not Found
            if (experienceEntity == null)
            {
                return NotFound();
            }

            // getting the moods felt during this experience
            var experienceMoods = await _context.ExperienceMoods
                .Where(em => em.ExperienceId == id)
                .Select(em => em.Mood.MoodName)
                .Distinct()
                .ToListAsync();

            // create an instance of experienceDto
            ExperienceDto experienceDto = new ExperienceDto()
            {
                ExperienceId = experienceEntity.ExperienceId,  
                ExperienceName = experienceEntity.ExperienceName,
                HobbyId = experienceEntity.HobbyId,
                HobbyName = experienceEntity.Hobby.HobbyName,
                ExperienceCost = experienceEntity.ExperienceCost,
                DurationinHours = experienceEntity.DurationinHours,
                ExperienceDate = experienceEntity.ExperienceDate,
                ExperienceLocation = experienceEntity.ExperienceLocation,
                ExperienceMoods = experienceMoods
            };

            // return 200 OK and experienceDto
            return Ok(experienceDto);
        }


        /// <summary>
        /// Adds a new experience.
        /// </summary>
        /// <param name="experienceDto">The required information to add the experience (ExperienceName, HobbyId, etc.)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Experience/Find/{ExperienceId}
        /// {ExperienceDto}
        /// or
        /// 400 Bad Request
        /// </returns>
        [HttpPost("Add")]
        public async Task<ActionResult<Experience>> AddExperience(ExperienceDto experienceDto)
        {
            // Validate hobby existence
            var hobby = await _context.Hobbies.FindAsync(experienceDto.HobbyId);
            if (hobby == null)
            {
                return NotFound();
            }

            // create new experience
            Experience experience = new Experience()
            {
                ExperienceName = experienceDto.ExperienceName,
                HobbyId = hobby.HobbyId,
                ExperienceCost = experienceDto.ExperienceCost,
                DurationinHours = experienceDto.DurationinHours,
                ExperienceDate = experienceDto.ExperienceDate,
                ExperienceLocation = experienceDto.ExperienceLocation
            };

            _context.Experiences.Add(experience);
            await _context.SaveChangesAsync();

            // should return 201 Created with Location
            return CreatedAtAction("FindExperience", new { id = experience.ExperienceId }, experienceDto);
        }

        /// <summary>
        /// Updates an existing experience.
        /// </summary>
        /// <param name="id">Experience ID</param>
        /// <param name="experienceDto">The required information to update the experience (ExperienceId, ExperienceName, etc.)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateExperience(int id, ExperienceDto experienceDto)
        {
            // id in url must match ExperienceId in POST body
            if (id != experienceDto.ExperienceId)
            {
                //otherwise 400 Bad Request
                return BadRequest();
            }

            // finding the existing experience that matches
            var existingExperience = await _context.Experiences.FindAsync(id);

            //if there is no existing experience
            if (existingExperience == null)
            {
                //404 Not Found
                return NotFound();
            }

            // Validate hobby existence
            var hobby = await _context.Hobbies.FirstOrDefaultAsync(h => h.HobbyName == experienceDto.HobbyName);
            if (hobby == null)
            {
                return NotFound();
            }

            // Update experience fields
            existingExperience.ExperienceName = experienceDto.ExperienceName;
            existingExperience.HobbyId = hobby.HobbyId;
            existingExperience.ExperienceCost = experienceDto.ExperienceCost;
            existingExperience.DurationinHours = experienceDto.DurationinHours;
            existingExperience.ExperienceDate = experienceDto.ExperienceDate;
            existingExperience.ExperienceLocation = experienceDto.ExperienceLocation;

            // flag that object has been changed
            _context.Entry(existingExperience).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExperienceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Checks if an experience exists by ID.
        /// </summary>
        /// <param name="id">Experience ID</param>
        /// <returns>True if experience exists, otherwise false.</returns>
        private bool ExperienceExists(int id)
        {
            return _context.Experiences.Any(e => e.ExperienceId == id);
        }

        /// <summary>
        /// Deletes an experience by ID.
        /// </summary>
        /// <param name="id">Experience ID</param>
        /// <returns>204 No Content, or 404 Not Found</returns>
        /// <example>DELETE: api/Experience/Delete/1</example>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            //checking if the experience exists
            var experience = await _context.Experiences.FindAsync(id);
            
            if (experience == null)
            {
                return NotFound();
            }

            // delete related ExperienceMoods 
            var experienceMoods = _context.ExperienceMoods.Where(em => em.ExperienceId == id);
            _context.ExperienceMoods.RemoveRange(experienceMoods);
            await _context.SaveChangesAsync();

            // delete the experience
            _context.Experiences.Remove(experience);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

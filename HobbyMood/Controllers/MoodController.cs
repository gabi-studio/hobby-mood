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
    public class MoodController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of database context
        public MoodController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of moods with experience count and top hobbies.
        /// </summary>
        /// <returns>200 OK with MoodDto List</returns>
        /// <example>GET: api/Mood/List => { "moodId": 1, "moodName": "Relaxed", "moodExperienceCount": 5, "topHobbies": ["Reading", "Gardening", "Puzzles"] }</example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<MoodDto>>> ListMoods()
        {
            List<Mood> moods = await _context.Moods.ToListAsync();

            // empty list of dto MoodDto
            List<MoodDto> moodDtos = new List<MoodDto>();

            foreach (Mood m in moods)
            {
                // creating an instance of MoodDto
                moodDtos.Add(new MoodDto()
                {
                    MoodId = m.MoodId,
                    MoodName = m.MoodName,

                    // counting the number of experiences associated with this mood
                    MoodExperienceCount = _context.ExperienceMoods
                        .Where(em => em.MoodId == m.MoodId)
                        .Select(em => em.ExperienceId)
                        .Distinct()
                        .Count(),

                    // getting the top 3 hobbies linked to this mood
                    TopHobbies = _context.ExperienceMoods
                        .Where(em => em.MoodId == m.MoodId)
                        .Select(em => em.Experience.Hobby.HobbyName)
                        .GroupBy(h => h)
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g => g.Key)
                        .ToList()
                });
            }

            return Ok(moodDtos);
        }


        /// <summary>
        /// Returns a specific mood by ID.
        /// </summary>
        /// <param name="id">Mood ID</param>
        /// <returns>200 OK with MoodDto, or 404 Not Found</returns>
        /// <example>GET: api/Mood/Find/1 => { "moodId": 1, "moodName": "Relaxed", "moodExperienceCount": 5, "topHobbies": ["Reading", "Gardening", "Puzzles"] }</example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<MoodDto>> FindMood(int id)
        {
            // get the first mood matching the {id} 
            var moodEntity = await _context.Moods.FindAsync(id);

            // if this mood can't be located, return 404 Not Found
            if (moodEntity == null)
            {
                return NotFound();
            }

            // counting the number of experiences linked to this mood
            var moodExperienceCount = await _context.ExperienceMoods
                .Where(em => em.MoodId == id)
                .Select(em => em.ExperienceId)
                .Distinct()
                .CountAsync();

            // getting the top 3 hobbies linked to this mood
            var topHobbies = await _context.ExperienceMoods
                .Where(em => em.MoodId == id)
                .Select(em => em.Experience.Hobby.HobbyName)
                .GroupBy(h => h)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToListAsync();

            // create an instance of MoodDto
            MoodDto moodDto = new MoodDto()
            {
                MoodId = moodEntity.MoodId,
                MoodName = moodEntity.MoodName,
                MoodExperienceCount = moodExperienceCount,
                TopHobbies = topHobbies
            };

            // return 200 OK and moodDto
            return Ok(moodDto);
        }


        /// <summary>
        /// Adds a new mood.
        /// </summary>
        /// <param name="moodDto">The required information to add the mood (MoodName)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Mood/Find/{MoodId}
        /// {MoodDto}
        /// or
        /// 400 Bad Request
        /// </returns>
        [HttpPost("Add")]
        public async Task<ActionResult<Mood>> AddMood(MoodDto moodDto)
        {
            // Validate that MoodName is not empty
            if (string.IsNullOrWhiteSpace(moodDto.MoodName))
            {
                return BadRequest();
            }

            // create a new Mood entity
            Mood mood = new Mood()
            {
                MoodName = moodDto.MoodName
            };

            _context.Moods.Add(mood);
            await _context.SaveChangesAsync();

            moodDto.MoodId = mood.MoodId;

            // should return 201 Created with Location
            return CreatedAtAction("FindMood", new { id = mood.MoodId }, moodDto);
        }

        /// <summary>
        /// Updates an existing mood.
        /// </summary>
        /// <param name="id">Mood ID</param>
        /// <param name="moodDto">The required information to update the mood (MoodId, MoodName)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateMood(int id, MoodDto moodDto)
        {
            // id in url must match MoodId in POST body
            if (id != moodDto.MoodId)
            {
                return BadRequest();
            }

            // finding the existing mood that matches
            var existingMood = await _context.Moods.FindAsync(id);

            // if there is no existing mood
            if (existingMood == null)
            {
                return NotFound();
            }

            // Update mood name
            existingMood.MoodName = moodDto.MoodName;

            // flag that object has been changed
            _context.Entry(existingMood).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MoodExists(id))
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
        /// Checks if a mood exists by ID.
        /// </summary>
        /// <param name="id">Mood ID</param>
        /// <returns>True if mood exists, otherwise false.</returns>
        private bool MoodExists(int id)
        {
            return _context.Moods.Any(m => m.MoodId == id);
        }

        /// <summary>
        /// Deletes a mood by ID.
        /// </summary>
        /// <param name="id">Mood ID</param>
        /// <returns>204 No Content, or 404 Not Found</returns>
        /// <example>DELETE: api/Mood/Delete/1</example>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteMood(int id)
        {
            // checking if the mood exists
            var mood = await _context.Moods.FindAsync(id);

            if (mood == null)
            {
                return NotFound();
            }

            // delete related ExperienceMood records 
            var experienceMoods = _context.ExperienceMoods.Where(em => em.MoodId == id);
            _context.ExperienceMoods.RemoveRange(experienceMoods);
            await _context.SaveChangesAsync();

            // delete the mood
            _context.Moods.Remove(mood);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

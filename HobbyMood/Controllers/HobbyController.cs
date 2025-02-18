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
    public class HobbyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of database context
        public HobbyController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of hobbies with their experience count, hours spent, and typical moods.
        /// </summary>
        /// <returns>200 OK with HobbyDto List</returns>
        /// <example>GET: api/Hobby/List</example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<HobbyDto>>> ListHobbies()
        {
            List<Hobby> hobbies = await _context.Hobbies.ToListAsync();
            List<HobbyDto> hobbyDtos = new List<HobbyDto>();

            foreach (Hobby h in hobbies)
            {
                hobbyDtos.Add(new HobbyDto()
                {
                    HobbyId = h.HobbyId,
                    HobbyName = h.HobbyName,

                    // numer of experiences with this hobby
                    NumberofExperiences = _context.Experiences.Where(e => e.HobbyId == h.HobbyId).Count(),

             
                    // sum of hours spent on experiences for each hobby
                    HoursSpent = _context.Experiences
                        .Where(e => e.HobbyId == h.HobbyId)
                        .ToList() 
                        .Sum(e => e.DurationinHours),

                    // getting the 3 most common moods experienced with each hobby
                    TypicalMoods = _context.ExperienceMoods
                        .Where(em => em.Experience.HobbyId == h.HobbyId)
                        .GroupBy(em => em.Mood.MoodName)
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g => g.Key)
                        .ToList()
                });
            }

            return Ok(hobbyDtos);
        }

        /// <summary>
        /// Returns a single hobby by its ID.
        /// </summary>
        /// <param name="id">Hobby ID</param>
        /// <returns>200 OK with HobbyDto, or 404 Not Found</returns>
        /// <example>GET: api/Hobby/FindHobby/1</example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<HobbyDto>> FindHobby(int id)
        {
            var hobbyEntity = await _context.Hobbies.FindAsync(id);

            if (hobbyEntity == null)
            {
                return NotFound();
            }

            // getting list of experiences
            var experiences = await _context.Experiences
                .Where(e => e.HobbyId == id)
                .ToListAsync();

            // the total hours spent on experiences with hobby
            decimal hoursSpent = experiences.Sum(e => e.DurationinHours); 

            // getting the 3 most common moods experienced with hobby
            var typicalMoods = await _context.ExperienceMoods
                .Where(em => em.Experience.HobbyId == id)
                .GroupBy(em => em.Mood.MoodName)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToListAsync();

            // hobby dto
            HobbyDto hobbyDto = new HobbyDto()
            {
                HobbyId = hobbyEntity.HobbyId,
                HobbyName = hobbyEntity.HobbyName,
                NumberofExperiences = experiences.Count, 
                HoursSpent = hoursSpent,
                TypicalMoods = typicalMoods
            };

            return Ok(hobbyDto);
        }



   

        /// <summary>
        /// Adds a new hobby
        /// </summary>
        /// <param name="hobbyDto">The required information to add the hobby (HobbyName)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/AddHobby/Find/{HobbyId}
        /// {HobbyDto}
        /// or
        /// 400 Bad Request
        /// </returns>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<Hobby>> AddHobby(HobbyDto hobbyDto)
        {
           

            // create a new Hobby entity
            Hobby hobby = new Hobby()
            {
                HobbyName = hobbyDto.HobbyName
            };

            

            _context.Hobbies.Add(hobby);
            await _context.SaveChangesAsync();

            //  adding the new hobby to the database
            _context.Hobbies.Add(hobby);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Find", new { id = hobby.HobbyId }, hobby);
        }

        /// <summary>
        /// Updates an existing hobby
        /// </summary>
        /// <param name="id">The id of Hobby to update</param>
        /// <param name="hobbyDto">The required information to update the hobby (HobbyId, HobbyName)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        [HttpPut(template: "Update/{id}")]
        public async Task<IActionResult> UpdateHobby(int id, HobbyDto hobbyDto)
        {
            // validating hobby id
            if (id != hobbyDto.HobbyId)
            {
                // 400 Bad Request
                return BadRequest();
            }

            // find existing hobby in db
            //var db = _context;
            var existingHobby = await _context.Hobbies.FindAsync(id);

            // aata must link to a valid existing entity
            if (existingHobby == null)
            {
                // 404 Not Found
                return NotFound();
            }

            // create a new instance of Hobby
            Hobby hobby = new Hobby()
            {
                HobbyId = hobbyDto.HobbyId,
                HobbyName = hobbyDto.HobbyName
            };

            // flags that the object has changed
            _context.Entry(hobby).State = EntityState.Modified;

            try
            {
                // update the db
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HobbyExists(id))
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
        /// Checks if a hobby exists by ID.
        /// </summary>
        /// <param name="id">Hobby ID</param>
        /// <returns>True if hobby exists, otherwise false.</returns>
        private bool HobbyExists(int id)
        {
            return _context.Hobbies.Any(e => e.HobbyId == id);
        }



        /// <summary>
        /// Deletes a hobby by ID.
        /// </summary>
        /// <param name="id">Hobby ID</param>
        /// <returns>204 No Content, or 404 Not Found</returns>
        /// <example>DELETE: api/Hobby/DeleteHobby/1</example>
        [HttpDelete(template: "Delete/{id}")]
        public async Task<IActionResult> DeleteHobby(int id)
        {
            

            Console.WriteLine($"The id is {id}");

            // find hobby with its related experiences
            var hobby = await _context.Hobbies.FindAsync(id);
            //.Include(e => e.Experiences)
            //.FirstOrDefaultAsync(h => h.HobbyId == id);

            if (hobby == null)
            {
                //Console.WriteLine($"The id {id} is not found");
                return NotFound();

            }

            //// sever the relationship by setting HobbyId in experiences to null
            ////reference: https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete
            //foreach (var experience in hobby.Experiences)
            //{
            //    experience.HobbyId = null; 
            //}

            //await db.SaveChangesAsync(); 

            // delete the hobby
            _context.Hobbies.Remove(hobby);
            await _context.SaveChangesAsync();

            //Console.WriteLine($"The hobby {id} has been deleted");
            //return 204 
            return NoContent();
        }



    }
}

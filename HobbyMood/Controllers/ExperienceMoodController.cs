using HobbyMood.Interfaces;
using HobbyMood.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HobbyMood.Data;

namespace HobbyMood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceMoodController : ControllerBase
    {
        private readonly IExperienceMoodService _experienceMoodService;

        // Dependency injection of service interfaces
        public ExperienceMoodController(IExperienceMoodService experienceMoodService)
        {
            _experienceMoodService = experienceMoodService;
        }

        /// <summary>
        /// Returns a list of all experience moods.
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ExperienceMoodDto},{ExperienceMoodDto},..]
        /// </returns>
        /// <example>
        /// GET: api/ExperienceMood/List -> [{ExperienceMoodDto},{ExperienceMoodDto},..]
        /// </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ExperienceMoodDto>>> ListExperienceMoods()
        {
            IEnumerable<ExperienceMoodDto> experienceMoodDtos = await _experienceMoodService.ListExperienceMoods();
            return Ok(experienceMoodDtos);
        }

        /// <summary>
        /// Returns a specific experience mood by ID.
        /// </summary>
        /// <param name="id">The Experience Mood ID</param>
        /// <returns>
        /// 200 OK
        /// {ExperienceMoodDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/ExperienceMood/Find/1 -> {ExperienceMoodDto}
        /// </example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ExperienceMoodDto>> FindExperienceMood(int id)
        {
            var experienceMoodDto = await _experienceMoodService.FindExperienceMood(id);

            if (experienceMoodDto == null)
            {
                return NotFound();
            }

            return Ok(experienceMoodDto);
        }

        /// <summary>
        /// Adds a new experience mood.
        /// </summary>
        /// <param name="experienceMoodDto">The required information to add the experience mood</param>
        /// <returns>
        /// 201 Created
        /// Location: api/ExperienceMood/Find/{ExperienceMoodId}
        /// {ExperienceMoodDto}
        /// or
        /// 500 Internal Server Error
        /// </returns>
        /// <example>
        /// POST: api/ExperienceMood/Add
        /// Request Body: {ExperienceMoodDto}
        /// -> Response Code: 201 Created
        /// Response Headers: Location: api/ExperienceMood/Find/{ExperienceMoodId}
        /// </example>
        [HttpPost("Add")]
        [Authorize]
        public async Task<ActionResult<ExperienceMood>> AddExperienceMood(ExperienceMoodDto experienceMoodDto)
        {
            ServiceResponse response = await _experienceMoodService.AddExperienceMood(experienceMoodDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Created($"api/ExperienceMood/Find/{response.CreatedId}", experienceMoodDto);
        }

        /// <summary>
        /// Updates an existing experience mood.
        /// </summary>
        /// <param name="id">The Experience Mood ID</param>
        /// <param name="experienceMoodDto">The required information to update the experience mood</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// POST: api/ExperienceMood/Update/5
        /// Request Body: {ExperienceMoodDto}
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpPost("Update/{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateExperienceMood(int id, ExperienceMoodDto experienceMoodDto)
        {
            if (id != experienceMoodDto.ExperienceMoodId)
            {
                return BadRequest();
            }

            ServiceResponse response = await _experienceMoodService.UpdateExperienceMood(id, experienceMoodDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes an experience mood by ID.
        /// </summary>
        /// <param name="id">The Experience Mood ID to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/ExperienceMood/Delete/7
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteExperienceMood(int id)
        {
            ServiceResponse response = await _experienceMoodService.DeleteExperienceMood(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Returns a list of experience moods for a specific experience.
        /// </summary>
        /// <param name="experienceId">The Experience ID</param>
        /// <returns>
        /// 200 OK
        /// [{ExperienceMoodDto},{ExperienceMoodDto},..]
        /// </returns>
        /// <example>
        /// GET: api/ExperienceMood/ListForExperience/3 -> [{ExperienceMoodDto},{ExperienceMoodDto},..]
        /// </example>
        [HttpGet("ListForExperience/{experienceId}")]
        public async Task<ActionResult<IEnumerable<ExperienceMoodDto>>> ListExperienceMoodsForExperience(int experienceId)
        {
            var experienceMoods = await _experienceMoodService.ListExperienceMoodsForExperience(experienceId);
            return Ok(experienceMoods);
        }

    }
}

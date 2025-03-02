using HobbyMood.Interfaces;
using HobbyMood.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HobbyMood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HobbyController : ControllerBase
    {
        private readonly IHobbyService _hobbyService;

        // Dependency injection of service interface
        public HobbyController(IHobbyService hobbyService)
        {
            _hobbyService = hobbyService;
        }


        /// <summary>
        /// Returns a list of Hobbies
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{HobbyDto},{HobbyDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Hobby/List -> [{HobbyDto},{HobbyDto},..]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<HobbyDto>>> ListHobbies()
        {
            IEnumerable<HobbyDto> hobbyDtos = await _hobbyService.ListHobbies();
            return Ok(hobbyDtos);
        }


        /// <summary>
        /// Returns a single Hobby specified by its {id}
        /// </summary>
        /// <param name="id">The Hobby id</param>
        /// <returns>
        /// 200 OK
        /// {HobbyDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Hobby/Find/1 -> {HobbyDto}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<HobbyDto>> FindHobby(int id)
        {
            var hobby = await _hobbyService.FindHobby(id);

            if (hobby == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(hobby);
            }
        }


        /// <summary>
        /// Returns a list of Experiences related to a specific Hobby
        /// </summary>
        /// <param name="hobbyId">The Hobby ID</param>
        /// <returns>
        /// 200 OK
        /// [{ExperienceDto},{ExperienceDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Hobby/Experiences/3 -> [{ExperienceDto},{ExperienceDto},..]
        /// </example>
        [HttpGet(template: "Experiences/{hobbyId}")]
        public async Task<IActionResult> ListExperiencesForHobby(int hobbyId)
        {
            IEnumerable<ExperienceDto> experienceDtos = await _hobbyService.ListExperiencesForHobby(hobbyId);
            return Ok(experienceDtos);
        }


        /// <summary>
        /// Adds a Hobby
        /// </summary>
        /// <param name="hobbyDto">The required information to add the hobby (HobbyName)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Hobby/Find/{HobbyId}
        /// {HobbyDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Hobby/Add
        /// Request Headers: Content-Type: application/json, cookie: .AspNetCore.Identity.Application={token}
        /// Request Body: {HobbyDto}
        /// -> Response Code: 201 Created
        /// Response Headers: Location: api/Hobby/Find/{HobbyId}
        /// </example>
        [HttpPost(template: "Add")]
        [Authorize]
        public async Task<ActionResult<Hobby>> AddHobby(HobbyDto hobbyDto)
        {
            ServiceResponse response = await _hobbyService.AddHobby(hobbyDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Hobby/Find/{response.CreatedId}", hobbyDto);
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
        /// <example>
        /// PUT: api/Hobby/Update/5
        /// Request Headers: Content-Type: application/json, cookie: .AspNetCore.Identity.Application={token}
        /// Request Body: {HobbyDto}
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpPut(template: "Update/{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateHobby(int id, HobbyDto hobbyDto)
        {
            if (id != hobbyDto.HobbyId)
            {
                return BadRequest();
            }

            ServiceResponse response = await _hobbyService.UpdateHobby(id, hobbyDto);

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
        /// Deletes the Hobby
        /// </summary>
        /// <param name="id">The id of the hobby to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Hobby/Delete/7
        /// Headers: cookie: .AspNetCore.Identity.Application={token}
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteHobby(int id)
        {
            ServiceResponse response = await _hobbyService.DeleteHobby(id);

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
        /// Links a Hobby to an Experience
        /// </summary>
        /// <param name="hobbyId">The id of the Hobby</param>
        /// <param name="experienceId">The id of the Experience</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Hobby/Link?HobbyId=4&ExperienceId=12
        /// /// Headers: cookie: .AspNetCore.Identity.Application={token}
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpPost("Link")]
        [Authorize]
        public async Task<ActionResult> LinkHobbyToExperience(int hobbyId, int experienceId)
        {
            ServiceResponse response = await _hobbyService.LinkHobbyToExperience(hobbyId, experienceId);

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
        /// Unlinks a Hobby from an Experience
        /// </summary>
        /// <param name="hobbyId">The id of the Hobby</param>
        /// <param name="experienceId">The id of the Experience</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Hobby/Unlink?HobbyId=4&ExperienceId=12
        /// Headers: cookie: .AspNetCore.Identity.Application={token}
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpDelete("Unlink")]
        [Authorize]
        public async Task<ActionResult> UnlinkHobbyFromExperience(int hobbyId, int experienceId)
        {
            ServiceResponse response = await _hobbyService.UnlinkHobbyFromExperience(hobbyId, experienceId);

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
    }
}

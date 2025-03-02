using HobbyMood.Interfaces;
using HobbyMood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HobbyMood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceService _experienceService;

        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }


        /// <summary>
        /// Returns a list of all Experiences
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ExperienceDto},{ExperienceDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Experience/List -> [{ExperienceDto},{ExperienceDto},..]
        /// </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ExperienceDto>>> ListExperiences()
        {
            var experiences = await _experienceService.ListExperiences();
            return Ok(experiences);
        }



        /// <summary>
        /// Returns a single Experience specified by its {id}
        /// </summary>
        /// <param name="id">The Experience ID</param>
        /// <returns>
        /// 200 OK
        /// {ExperienceDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Experience/Find/1 -> {ExperienceDto}
        /// </example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ExperienceDto>> FindExperience(int id)
        {
            var experience = await _experienceService.FindExperience(id);
            if (experience == null) return NotFound();

            return Ok(experience);
        }


        /// <summary>
        /// Adds an Experience
        /// </summary>
        /// <param name="experienceDto">The required information to add the experience</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Experience/Find/{ExperienceId}
        /// {ExperienceDto}
        /// or
        /// 500 Internal Server Error
        /// </returns>
        /// <example>
        /// POST: api/Experience/Add
        /// Request Headers: Content-Type: application/json, cookie: .AspNetCore.Identity.Application={token}
        /// Request Body: {ExperienceDto}
        /// -> Response Code: 201 Created
        /// Response Headers: Location: api/Experience/Find/{ExperienceId}
        /// </example>
        [HttpPost("Add")]
        [Authorize]
        public async Task<ActionResult> AddExperience(ExperienceDto experienceDto)
        {
            var response = await _experienceService.AddExperience(experienceDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Created($"api/Experience/Find/{response.CreatedId}", experienceDto);
        }


        /// <summary>
        /// Updates an existing Experience
        /// </summary>
        /// <param name="id">The ID of the Experience to update</param>
        /// <param name="experienceDto">The required information to update the experience</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Experience/Update/5
        /// Request Headers: Content-Type: application/json, cookie: .AspNetCore.Identity.Application={token} 
        /// Request Body: {ExperienceDto}
        /// -> Response Code: 204 No Content
        /// </example>

        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateExperience(int id, ExperienceDto experienceDto)
        {
            if (id != experienceDto.ExperienceId) return BadRequest();

            var response = await _experienceService.UpdateExperience(id, experienceDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound) return NotFound();
            if (response.Status == ServiceResponse.ServiceStatus.Error) return StatusCode(500, response.Messages);

            return NoContent();
        }


        /// <summary>
        /// Deletes an Experience
        /// </summary>
        /// <param name="id">The ID of the Experience to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Experience/Delete/7
        /// Request Headers: C cookie: .AspNetCore.Identity.Application={token}
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteExperience(int id)
        {
            var response = await _experienceService.DeleteExperience(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound) return NotFound();
            if (response.Status == ServiceResponse.ServiceStatus.Error) return StatusCode(500, response.Messages);

            return NoContent();
        }



        /// <summary>
        /// Updates an Experience's image and saves it to a designated location
        /// </summary>
        /// <param name="id">The Experience ID for which the image is being updated</param>
        /// <param name="experienceImage">The new image file</param>
        /// <returns>
        /// 200 OK
        /// or
        /// 404 Not Found
        /// or
        /// 500 Internal Server Error
        /// </returns>
        /// <example>
        /// PUT: api/Experience/UpdateExperienceImage/5
        /// HEADERS: Content-Type: Multi-part/form-data, Cookie: .AspNetCore.Identity.Application={token}
        /// FORM DATA:
        /// ------boundary
        /// Content-Disposition: form-data; name="experienceImage"; filename="experience1.jpg"
        /// Content-Type: image/jpeg
        /// </example>
        /// <example>
        /// curl "https://localhost:xx/api/Experience/UpdateExperienceImage/5" -H "Cookie: .AspNetCore.Identity.Application={token}" -X "PUT" -F experienceImage=@experienve1.jpg
        /// </example>
        [HttpPut("UpdateExperienceImage/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateExperienceImage(int id, IFormFile experienceImage)
        {
            ServiceResponse response = await _experienceService.UpdateExperienceImage(id, experienceImage);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Ok(new { message = "Image updated successfully." });
        }

    }
}

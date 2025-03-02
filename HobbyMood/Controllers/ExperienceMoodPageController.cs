using Microsoft.AspNetCore.Mvc;
using HobbyMood.Interfaces;
using HobbyMood.Models;
using HobbyMood.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace HobbyMood.Controllers
{
    public class ExperienceMoodPageController : Controller
    {
        private readonly IHobbyService _hobbyService;
        private readonly IExperienceService _experienceService;
        private readonly IMoodService _moodService;
        private readonly IExperienceMoodService _experienceMoodService;

        public ExperienceMoodPageController(
            IHobbyService hobbyService,
            IExperienceService experienceService,
            IMoodService moodService,
            IExperienceMoodService experienceMoodService)
        {
            _hobbyService = hobbyService;
            _experienceService = experienceService;
            _moodService = moodService;
            _experienceMoodService = experienceMoodService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ExperienceMoodPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<ExperienceMoodDto> experienceMoodDtos = await _experienceMoodService.ListExperienceMoods();
            return View(experienceMoodDtos);
        }

        // GET: ExperienceMoodPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ExperienceMoodDto? experienceMoodDto = await _experienceMoodService.FindExperienceMood(id);

            if (experienceMoodDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find ExperienceMood"] });
            }

            return View(experienceMoodDto);
        }

        // GET: ExperienceMoodPage/New
        [Authorize]
        public async Task<IActionResult> New()
        {
            var experiences = await _experienceService.ListExperiences();
            var moods = await _moodService.ListMoods();

            ExperienceMoodNew options = new()
            {
                AllExperiences = experiences,
                AllMoods = moods
            };

            return View(options);
        }

        // POST: ExperienceMoodPage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(ExperienceMoodDto experienceMoodDto)
        {
            ServiceResponse response = await _experienceMoodService.AddExperienceMood(experienceMoodDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: ExperienceMoodPage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            ExperienceMoodDto? experienceMoodDto = await _experienceMoodService.FindExperienceMood(id);
            var experiences = await _experienceService.ListExperiences();
            var moods = await _moodService.ListMoods();

            if (experienceMoodDto == null)
            {
                return View("Error");
            }

            ExperienceMoodEdit options = new()
            {
                ExperienceMood = experienceMoodDto,
                AllExperiences = experiences,
                AllMoods = moods
            };

            return View(options);
        }

        // POST: ExperienceMoodPage/UpdateExperienceMood/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateExperienceMood(int id, [FromForm] ExperienceMoodDto experienceMoodDto)
        {
            if (id != experienceMoodDto.ExperienceMoodId)
            {
                return BadRequest(new { message = "ExperienceMood ID mismatch." });
            }

            var response = await _experienceMoodService.UpdateExperienceMood(id, experienceMoodDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { message = "ExperienceMood not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return BadRequest(new { message = "Error updating ExperienceMood." });
            }

            return RedirectToAction("Edit", new { success = true, message = "Mood updated successfully!" });
        }








        // GET: ExperienceMoodPage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            ExperienceMoodDto? experienceMoodDto = await _experienceMoodService.FindExperienceMood(id);

            if (experienceMoodDto == null)
            {
                return View("Error");
            }

            return View(experienceMoodDto);
        }

        // POST: ExperienceMoodPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _experienceMoodService.DeleteExperienceMood(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
    }
}

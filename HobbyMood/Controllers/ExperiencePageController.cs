using Microsoft.AspNetCore.Mvc;
using HobbyMood.Interfaces;
using HobbyMood.Models.ViewModels;
using HobbyMood.Models;
using Microsoft.AspNetCore.Authorization;

namespace HobbyMood.Controllers
{
    public class ExperiencePageController : Controller
    {
        private readonly IExperienceService _experienceService;
        private readonly IHobbyService _hobbyService;
        private readonly IMoodService _moodService;
        private readonly IExperienceMoodService _experienceMoodService;

        // Dependency injection of services
        public ExperiencePageController(
            IExperienceService experienceService, 
            IHobbyService hobbyService, 
            IMoodService moodService, 
            IExperienceMoodService experienceMoodService)
        {
            _experienceService = experienceService;
            _hobbyService = hobbyService;
            _moodService = moodService;
            _experienceMoodService = experienceMoodService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ExperiencePage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<ExperienceDto> experiences = await _experienceService.ListExperiences();
            return View(experiences);
        }

        // GET: ExperiencePage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ExperienceDto? experience = await _experienceService.FindExperience(id);

            if (experience == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Experience not found."] });
            }

            // All hobbies
            IEnumerable<HobbyDto> hobbyOptions = await _hobbyService.ListHobbies();

            //  all moods
            IEnumerable<MoodDto> moodOptions = await _moodService.ListMoods();

            //  all moods already linked to this experience
            IEnumerable<ExperienceMoodDto> experienceMoods = await _experienceMoodService.ListExperienceMoodsForExperience(id);

            // Pass all data to the ViewModel
            ExperienceDetails experienceInfo = new ExperienceDetails()
            {
                Experience = experience,
                HobbyOptions = hobbyOptions,
                MoodOptions = moodOptions,
                ExperienceMoods = experienceMoods
            };

            return View(experienceInfo);
        }

        // GET: ExperiencePage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            // Retrieve the experience by ID
            ExperienceDto? experience = await _experienceService.FindExperience(id);

            // Get all available hobbies and moods
            IEnumerable<HobbyDto> hobbies = await _hobbyService.ListHobbies();
            IEnumerable<MoodDto> moods = await _moodService.ListMoods();
            
            
            IEnumerable<ExperienceMoodDto> experienceMoods = await _experienceMoodService.ListExperienceMoodsForExperience(id);

            // If experience is not found, return an error page
            if (experience == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Experience not found."] });
            }

            
            ExperienceDetails experienceInfo = new ExperienceDetails()
            {
                Experience = experience,
                HobbyOptions = hobbies,
                MoodOptions = moods,
                ExperienceMoods = experienceMoods
            };

            return View(experienceInfo);
        }


        // POST: ExperiencePage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, ExperienceDto experienceDto)
        {
            if (id != experienceDto.ExperienceId)
            {
                experienceDto.ExperienceId = id; 
            }

            ServiceResponse response = await _experienceService.UpdateExperience(id, experienceDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }


        // POST: ExperiencePage/UpdateHobby
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateHobby(int experienceId, int hobbyId)
        {
            ServiceResponse response = await _experienceService.LinkExperienceToHobby(experienceId, hobbyId);

            return RedirectToAction("Details", new { id = experienceId });
        }

        // POST: ExperiencePage/LinkMood
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LinkMood(int experienceId, int moodId, int? beforeIntensity, int? afterIntensity)
        {
            ServiceResponse response = await _experienceMoodService.LinkExperienceToMood(experienceId, moodId, beforeIntensity, afterIntensity);

            return RedirectToAction("Details", new { id = experienceId });
        }

        // POST: ExperiencePage/UnlinkMood
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnlinkMood(int experienceMoodId, int experienceId)
        {
            ServiceResponse response = await _experienceMoodService.UnlinkExperienceFromMood(experienceMoodId);

            return RedirectToAction("Details", new { id = experienceId });
        }


        // POST: ExperienceMoodPage/UpdateExperienceMood/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateExperienceMood(int id, ExperienceMoodDto experienceMoodDto)
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



        // GET: ExperiencePage/New
        [Authorize]
        public async Task<IActionResult> New()
        {
            IEnumerable<HobbyDto> hobbies = await _hobbyService.ListHobbies();
            IEnumerable<MoodDto> moods = await _moodService.ListMoods();

            ExperienceNew experienceNew = new ExperienceNew()
            {
                HobbyOptions = hobbies,
                MoodOptions = moods
            };

            return View(experienceNew);
        }

        // POST: ExperiencePage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(ExperienceDto experienceDto)
        {
            ServiceResponse response = await _experienceService.AddExperience(experienceDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "ExperiencePage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: ExperiencePage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            ExperienceDto? experience = await _experienceService.FindExperience(id);
            if (experience == null)
            {
                return View("Error");
            }
            else
            {
                return View(experience);
            }
        }

        // POST: ExperiencePage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _experienceService.DeleteExperience(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "ExperiencePage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateExperienceImage(int id, IFormFile experienceImage)
        {
            if (id <= 0)
            {
                return View("Error", new ErrorViewModel() { Errors = [$"Invalid Experience ID: {id}"] });
            }

            ServiceResponse response = await _experienceService.UpdateExperienceImage(id, experienceImage);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }


    }
}

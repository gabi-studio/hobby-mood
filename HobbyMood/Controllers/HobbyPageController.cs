using Microsoft.AspNetCore.Mvc;
using HobbyMood.Interfaces;
using HobbyMood.Models;
using HobbyMood.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HobbyMood.Controllers
{
    public class HobbyPageController : Controller
    {
        private readonly IHobbyService _hobbyService;
        private readonly IExperienceService _experienceService;
        private readonly IMoodService _moodService;
        private readonly IExperienceMoodService _experienceMoodService;

        // Dependency injection of service interfaces
        public HobbyPageController(
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

        // GET: HobbyPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<HobbyDto?> hobbyDtos = await _hobbyService.ListHobbies();
            return View(hobbyDtos);
        }

        // GET: HobbyPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            HobbyDto? hobby = await _hobbyService.FindHobby(id);
            IEnumerable<ExperienceDto> associatedExperiences = await _experienceService.ListExperiencesForHobby(id);
            IEnumerable<MoodDto> allMoods = await _moodService.ListMoods();
            IEnumerable<ExperienceMoodDto> experienceMoods = await _experienceMoodService.ListExperienceMoods();

            if (hobby == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Sorry, Hobby not found."] });
            }
            else
            {
                HobbyDetails hobbyDetails = new HobbyDetails()
                {
                    Hobby = hobby,
                    HobbyExperiences = associatedExperiences,
                    AllMoods = allMoods,
                    ExperienceMoods = experienceMoods
                };
                return View(hobbyDetails);
            }
        }

        // GET: HobbyPage/New

        public ActionResult New()
        {
            return View();
        }

        // POST: HobbyPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(HobbyDto hobbyDto)
        {
            ServiceResponse response = await _hobbyService.AddHobby(hobbyDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "HobbyPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: HobbyPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            HobbyDto? hobby = await _hobbyService.FindHobby(id);
            if (hobby == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Hobby not found."] });
            }
            return View(hobby);  
        }


        // POST: HobbyPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, HobbyDto hobbyDto)
        {
            if (id != hobbyDto.HobbyId)  
            {
                return View("Error", new ErrorViewModel() { Errors = ["Hobby ID mismatch."] });
            }

            ServiceResponse response = await _hobbyService.UpdateHobby(id, hobbyDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "HobbyPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }



        // GET: HobbyPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            HobbyDto? hobbyDto = await _hobbyService.FindHobby(id);
            if (hobbyDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(hobbyDto);
            }
        }

        // POST: HobbyPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _hobbyService.DeleteHobby(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "HobbyPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // POST: HobbyPage/LinkToExperience
        [HttpPost]
        public async Task<IActionResult> LinkToExperience([FromForm] int hobbyId, [FromForm] int experienceId)
        {
            await _hobbyService.LinkHobbyToExperience(hobbyId, experienceId);

            return RedirectToAction("Details", new { id = hobbyId });
        }

        // POST: HobbyPage/UnlinkFromExperience
        [HttpPost]
        public async Task<IActionResult> UnlinkFromExperience([FromForm] int hobbyId, [FromForm] int experienceId)
        {
            await _hobbyService.UnlinkHobbyFromExperience(hobbyId, experienceId);

            return RedirectToAction("Details", new { id = hobbyId });
        }
    }
}

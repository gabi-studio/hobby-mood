using Microsoft.AspNetCore.Mvc;
using HobbyMood.Interfaces;
using HobbyMood.Models.ViewModels;
using HobbyMood.Models;
using Microsoft.AspNetCore.Authorization;

namespace HobbyMood.Controllers
{
    public class MoodPageController : Controller
    {
        private readonly IMoodService _moodService;
        private readonly IExperienceService _experienceService;

        public MoodPageController(IMoodService moodService, IExperienceService experienceService)
        {
            _moodService = moodService;
            _experienceService = experienceService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: MoodPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<MoodDto?> moodDtos = await _moodService.ListMoods();
            return View(moodDtos);
        }

        // GET: MoodPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            MoodDto? moodDto = await _moodService.FindMood(id);
            IEnumerable<ExperienceDto> relatedExperiences = await _experienceService.ListExperiencesForMood(id);

            if (moodDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find mood"] });
            }

            MoodDetails moodInfo = new MoodDetails()
            {
                Mood = moodDto,
                RelatedExperiences = relatedExperiences
            };

            return View(moodInfo);
        }

        // GET: MoodPage/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: MoodPage/Add
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(MoodDto moodDto)
        {
            ServiceResponse response = await _moodService.AddMood(moodDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { id = response.CreatedId });
            }
            return View("Error", new ErrorViewModel() { Errors = response.Messages });
        }

        // GET: MoodPage/Edit/{id}
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            MoodDto? moodDto = await _moodService.FindMood(id);
            if (moodDto == null)
            {
                return View("Error");
            }
            return View(moodDto);
        }

        // POST: MoodPage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, MoodDto moodDto)
        {
            ServiceResponse response = await _moodService.UpdateMood(id, moodDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id });
            }
            return View("Error", new ErrorViewModel() { Errors = response.Messages });
        }

        // GET: MoodPage/ConfirmDelete/{id}
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            MoodDto? moodDto = await _moodService.FindMood(id);
            if (moodDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(moodDto);
            }
        }

        // POST: MoodPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _moodService.DeleteMood(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "MoodPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

    }
}

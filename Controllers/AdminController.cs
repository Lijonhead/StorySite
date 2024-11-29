using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StorySite.Models;

namespace StorySite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Admin/Index
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.GetAsync("/api/User");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to fetch users.";
                return RedirectToAction("Index", "Home");
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(string userId)
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");

            // Fetch the user's profile
            var profileResponse = await client.GetAsync($"/api/Profile/user/{userId}");
            if (!profileResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to fetch user profile.";
                return RedirectToAction("Users");
            }
            var profile = await profileResponse.Content.ReadFromJsonAsync<ProfileViewModel>();

            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> UserPostHistory(string userId)
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");

            // Fetch the user's prompts
            var promptsResponse = await client.GetAsync($"/api/Prompt/user/{userId}");
            var storiesResponse = await client.GetAsync($"/api/Story/user/{userId}");

            if (!promptsResponse.IsSuccessStatusCode || !storiesResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to fetch user post history.";
                return RedirectToAction("Users");
            }

            var prompts = await promptsResponse.Content.ReadFromJsonAsync<List<PromptViewModel>>();
            var stories = await storiesResponse.Content.ReadFromJsonAsync<List<StoryViewModel>>();

            var postHistory = new UserPostHistoryViewModel
            {
                UserId = userId,
                Prompts = prompts,
                Stories = stories
            };

            return View(postHistory);
        }
        // POST: Admin/DeleteStory/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteStory(int id)
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.DeleteAsync($"/api/Story/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to delete the story.";
            }
            else
            {
                TempData["SuccessMessage"] = "Story deleted successfully.";
            }

            return RedirectToAction("Users", "Admin");
        }
        // GET: Admin/EditStory/{id}
        [HttpGet]
        public async Task<IActionResult> EditStory(int id)
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.GetAsync($"/api/Story/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to fetch the story.";
                return RedirectToAction("PostHistory", "Admin");
            }

            var story = await response.Content.ReadFromJsonAsync<StoryViewModel>();
            return View(story);
        }
        // POST: Admin/EditStory
        [HttpPost]
        public async Task<IActionResult> EditStory(StoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.PutAsJsonAsync($"/api/Story/{model.Id}", model);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to update the story.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Story updated successfully.";
            return RedirectToAction("Users", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.DeleteAsync($"/api/User/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
                return RedirectToAction("Users");
            }

            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction("Users");
        }
    }
}

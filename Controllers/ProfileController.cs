using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StorySite.Models;
using System.Security.Claims;

namespace StorySite.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        
        
        
            private readonly IHttpClientFactory _httpClientFactory;

            public ProfileController(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

        // GET: /Profile/Details
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var userId = User.FindFirst("sub")?.Value; // Extract user ID from token
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.GetAsync($"/api/Profile/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to fetch profile.";
                return RedirectToAction("Index", "Home");
            }

            var profile = await response.Content.ReadFromJsonAsync<ProfileViewModel>();
            return View(profile);
        }

        // GET: /Profile/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirst("sub")?.Value; // Ensure your token contains "sub"
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.GetAsync($"/api/Profile/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to fetch profile.";
                return RedirectToAction("Details");
            }

            var profile = await response.Content.ReadFromJsonAsync<ProfileViewModel>();

            // Add UserId and UserName from claims if not already in the profile
            profile.UserId = userId;
            profile.UserName = User.FindFirst("unique_name")?.Value ?? "Unknown";

            return View(profile);

        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            // Prepare UpdateProfileDTO
            var updateProfileDto = new UpdateProfileViewModel
            {
                Id = model.Id,
                UserId = model.UserId,
                Description = model.Description,
                Picture = model.Picture
            };

            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.PutAsJsonAsync($"/api/Profile/{model.UserId}", updateProfileDto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to update profile.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Details");
        }

        [HttpGet]
        public async Task<IActionResult> PostHistory()
        {
            var userId = User.FindFirst("sub")?.Value; // Fetch the logged-in user's ID
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("StoryPromptAPI");

            // Fetch user's prompts
            var promptsResponse = await client.GetAsync($"/api/Prompt/user/{userId}");
            var prompts = promptsResponse.IsSuccessStatusCode
                ? await promptsResponse.Content.ReadFromJsonAsync<List<PromptViewModel>>()
                : new List<PromptViewModel>();

            // Fetch user's stories
            var storiesResponse = await client.GetAsync($"/api/Story/user/{userId}");
            var stories = storiesResponse.IsSuccessStatusCode
                ? await storiesResponse.Content.ReadFromJsonAsync<List<StoryViewModel>>()
                : new List<StoryViewModel>();

            // Combine and order by date
            var posts = prompts.Select(p => new UserPostViewModel
            {
                Id = p.Id,
                Content = p.PromptContent,
                DateCreated = p.PromptDateCreated,
                Type = "Prompt"
            })
            .Concat(stories.Select(s => new UserPostViewModel
            {
                Id = s.Id,
                Content = s.StoryContent,
                DateCreated = s.StoryDateCreated,
                Type = "Story"
            }))
            .OrderByDescending(post => post.DateCreated)
            .ToList();

            return View(posts);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> DetailsOthers(string userId, string userName)
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.GetAsync($"/api/Profile/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to fetch profile.";
                return RedirectToAction("Index", "Home");
            }

            var profile = await response.Content.ReadFromJsonAsync<ProfileViewModel>();

            var profileCreate = new ProfileViewModel
            {
                Id = profile.Id,
                UserId = userId,
                UserName = userName,
                Description = profile.Description,
                Picture = profile.Picture,
                ProfileCreated = profile.ProfileCreated,

            };
           
            return View(profileCreate);
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using StorySite.Models;
using System.Reflection;

namespace StorySite.Controllers
{
    public class PromptController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PromptController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Prompt/Create
        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpGet]
        public IActionResult CreateDailyPrompt()

        {

            return View();
        }

        // POST: /Prompt/Create
        [HttpPost]
        public async Task<IActionResult> Create(PromptViewModel model)
        {
            var usersName = User.FindFirst("unique_name")?.Value;
            model.UserName = usersName ?? string.Empty;
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
            

            var userId = User.FindFirst("sub")?.Value; // Ensure this matches your token claims
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var createPrompt = new CreatePromptViewModel
            {

                UserId = userId,
                PromptContent = model.PromptContent
                
                
            };

            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.PostAsJsonAsync("/api/Prompt", createPrompt);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to create prompt.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Prompt created successfully!";
            return RedirectToAction("List");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateDailyPrompt(PromptViewModel model)
        {
            var usersName = User.FindFirst("unique_name")?.Value;
            model.UserName = usersName ?? string.Empty;
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

            // Prepend DailyPrompt: to the content for daily prompts
            
            var userId = User.FindFirst("sub")?.Value;
            var createPrompt = new CreatePromptViewModel
            {

                UserId = userId,
                PromptContent = "DailyPrompt: " + model.PromptContent


            };
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.PostAsJsonAsync("/api/Prompt", createPrompt);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to create daily prompt.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Daily prompt created successfully!";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Prompt/List
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.GetAsync("/api/Prompt");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to fetch prompts.";
                return RedirectToAction("Index", "Home");
            }

            var prompts = await response.Content.ReadFromJsonAsync<List<PromptViewModel>>();
            var promptDetailsList = new List<PromptDetailsViewModel>();
            foreach (var prompt in prompts)
            {
                // Fetch username
                var userResponse = await client.GetAsync($"/api/User/{prompt.UserId}");
                var username = await userResponse.Content.ReadFromJsonAsync<UserViewModel>();
                prompt.UserName = username?.UserName ?? "Unknown";

                // Fetch reactions
                var reactionsResponse = await client.GetAsync($"/api/PromptReaction/prompt/{prompt.Id}");
                var reactions = reactionsResponse.IsSuccessStatusCode
                    ? await reactionsResponse.Content.ReadFromJsonAsync<List<PromptReactionViewModel>>()
                    : new List<PromptReactionViewModel>();

                // Count likes and dislikes
                var likes = reactions.Count(r => r.Reaction == "Like");
                var dislikes = reactions.Count(r => r.Reaction == "Dislike");

                // Populate PromptDetailsViewModel
                promptDetailsList.Add(new PromptDetailsViewModel
                {
                    Id = prompt.Id,
                    PromptContent = prompt.PromptContent,
                    PromptDateCreated = prompt.PromptDateCreated,
                    UserName = prompt.UserName,
                    Likes = likes,
                    Dislikes = dislikes,
                    UserId = prompt.UserId
                });
            }
            var usersName = User.FindFirst("role")?.Value;
           
            

            return View(promptDetailsList);
        }

        // GET: /Prompt/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var promptResponse = await client.GetAsync($"/api/Prompt/{id}");

            if (!promptResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to load prompt.";
                return RedirectToAction("List");
            }

            var prompt = await promptResponse.Content.ReadFromJsonAsync<PromptDetailsViewModel>();

            var userResponse = await client.GetAsync($"/api/User/{prompt.UserId}");
            var username = await userResponse.Content.ReadFromJsonAsync<UserViewModel>();
            prompt.UserName = username?.UserName ?? "Unknown";

            var storiesResponse = await client.GetAsync($"/api/Story/prompt/{id}");
            var stories = storiesResponse.IsSuccessStatusCode
                ? await storiesResponse.Content.ReadFromJsonAsync<List<StoryViewModel>>()
                : new List<StoryViewModel>();
            // Map usernames for the stories
            foreach (var story in stories)
            {
                var storyResponse = await client.GetAsync($"/api/User/{story.UserId}");
                var storyReactionResponse = await client.GetAsync($"/api/StoryReaction/story/{story.Id}");
                var storyReactions = await storyReactionResponse.Content.ReadFromJsonAsync<List<StoryReactionViewModel>>();
                story.Likes = storyReactions.Count(r => r.Reaction == "Like");
                story.Dislikes = storyReactions.Count(r => r.Reaction == "Dislike");
                var storyUseName = await storyResponse.Content.ReadFromJsonAsync<UserViewModel>();
                story.UserName = storyUseName?.UserName ?? "Unknown";

            }
            // Fetch reactions
            var reactionsResponse = await client.GetAsync($"/api/PromptReaction/prompt/{prompt.Id}");
            var reactions = reactionsResponse.IsSuccessStatusCode
                ? await reactionsResponse.Content.ReadFromJsonAsync<List<PromptReactionViewModel>>()
                : new List<PromptReactionViewModel>();

            // Count likes and dislikes
            var likes = reactions.Count(r => r.Reaction == "Like");
            var dislikes = reactions.Count(r => r.Reaction == "Dislike");



            // Create the PromptDetailsViewModel
            var promptDetails = new PromptDetailsViewModel
            {
                Id = prompt.Id,
                PromptContent = prompt.PromptContent,
                PromptDateCreated = prompt.PromptDateCreated,
                UserName = prompt.UserName,
                Stories = stories,
                Likes = likes,
                Dislikes = dislikes,
                UserId = prompt.UserId
                
            };
            

            return View(promptDetails);
        }

        // GET: Prompt/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.GetAsync($"/api/Prompt/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to fetch the prompt.";
                return RedirectToAction("List");
            }

            var prompt = await response.Content.ReadFromJsonAsync<PromptEditViewModel>();
            if (prompt.UserId == User.FindFirst("sub")?.Value || User.IsInRole("Admin"))
            {
               return View(prompt);
            }
            TempData["ErrorMessage"] = "You are not authorized to edit this prompt.";
            return RedirectToAction("List");

            
        }
        // POST: Prompt/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(PromptEditViewModel model)
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

            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.PutAsJsonAsync($"/api/Prompt/{model.Id}", model);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to update the prompt.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Prompt updated successfully!";
            return RedirectToAction("List");
        }

        // POST: Prompt/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");

            // Check if the prompt belongs to the current user
            var response = await client.GetAsync($"/api/Prompt/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to fetch the prompt.";
                return RedirectToAction("List");
            }

            var prompt = await response.Content.ReadFromJsonAsync<PromptDetailsViewModel>();
            if ( prompt.UserId == User.FindFirst("sub")?.Value || User.IsInRole("Admin"))
            {
                // Call API to delete the prompt
                var deleteResponse = await client.DeleteAsync($"/api/Prompt/{id}");
                if (!deleteResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to delete the prompt.";
                    return RedirectToAction("List");
                }

                TempData["SuccessMessage"] = "Prompt deleted successfully!";
                return RedirectToAction("List");
            }
            TempData["ErrorMessage"] = "You are not authorized to delete this prompt.";
            return RedirectToAction("List");
            
        }

        [Authorize]
        public async Task<IActionResult> DailyPrompt()
        {
            var client = _httpClientFactory.CreateClient("StoryPromptAPI");
            var response = await client.GetAsync("/api/Prompt");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to load prompts.";
                Console.WriteLine(response);
                return View(new List<PromptViewModel>());
            }

            var prompts = await response.Content.ReadFromJsonAsync<List<PromptViewModel>>();

            // Fetch all users to determine roles
            var userResponse = await client.GetAsync("/api/User");
            if (!userResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to load user data.";
                return View(new List<PromptViewModel>());
            }

            var users = await userResponse.Content.ReadFromJsonAsync<List<UserViewModel>>();

            // Filter for daily prompts
            var dailyPrompts = prompts
                .Where(p =>
                {
                    var user = users.FirstOrDefault(u => u.Id == p.UserId);
                    return user != null && user.Roles.Contains("Admin") && p.PromptContent.StartsWith("DailyPrompt:");
                })
                .OrderBy(p => p.PromptDateCreated).FirstOrDefault();

            var theName = await client.GetAsync($"/api/User/{dailyPrompts.UserId}");
            var username = await theName.Content.ReadFromJsonAsync<UserViewModel>();

            var promptDetails = new PromptViewModel
            {
                Id = dailyPrompts.Id,
                PromptContent = dailyPrompts.PromptContent,
                PromptDateCreated = dailyPrompts.PromptDateCreated,
                UserName = username.UserName,                              
                UserId = dailyPrompts.UserId

            };

            return View(promptDetails);
        }
        
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StorySite.Models;

namespace StorySite.Controllers
{
    [Authorize]
    public class StoryReactionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StoryReactionController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> ReactToStory(int storyId, string reactionType, int promptId) // "Like" or "Dislike"
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("StoryPromptAPI");

            // Check if the user already has a reaction
            var response = await client.GetAsync($"/api/StoryReaction/user/{userId}");
            var existingReactions = await response.Content.ReadFromJsonAsync<List<StoryReactionViewModel>>();

            var existingReaction = existingReactions?.FirstOrDefault(r => r.StoryId == storyId);

            if (existingReaction != null)
            {
                // Update reaction if it already exists
                existingReaction.Reaction = reactionType;
                var updateResponse = await client.PutAsJsonAsync($"/api/StoryReaction/{existingReaction.Id}", existingReaction);
                if (!updateResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to update reaction.";
                }
            }
            else
            {
                // Create a new reaction
                var createReaction = new CreateStoryReactionViewModel
                {
                    Reaction = reactionType,
                    StoryId = storyId,
                    UserId = userId
                };
                var createResponse = await client.PostAsJsonAsync("/api/StoryReaction", createReaction);
                if (!createResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to add reaction.";
                }
            }

            return RedirectToAction("Details", "Prompt", new { id = promptId }); // Return to the story's prompt
        }
    }
}

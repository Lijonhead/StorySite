using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StorySite.Models;

namespace StorySite.Controllers
{
    [Authorize]
    public class PromptReactionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PromptReactionController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> React(int promptId, string reaction)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("StoryPromptAPI");

            // Fetch the existing reaction by the user for the prompt
            var existingReactionResponse = await client.GetAsync($"/api/PromptReaction/user/{userId}");
            var existingReactions = await existingReactionResponse.Content.ReadFromJsonAsync<IEnumerable<PromptReactionViewModel>>();

            var existingReaction = existingReactions?.FirstOrDefault(r => r.PromptId == promptId);

            if (existingReaction != null)
            {
                // Update reaction if it exists
                var updateReaction = new UpdatePromptReactionViewModel
                {
                    Id = existingReaction.Id,
                    Reaction = reaction,
                    PromptId = promptId,
                    UserId = userId
                };

                var updateResponse = await client.PutAsJsonAsync($"/api/PromptReaction/{existingReaction.Id}", updateReaction);
                if (!updateResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to update your reaction.";
                }
            }
            else
            {
                // Add new reaction if none exists
                var createReaction = new CreatePromptReactionViewModel
                {
                    Reaction = reaction,
                    PromptId = promptId,
                    UserId = userId
                };

                var createResponse = await client.PostAsJsonAsync("/api/PromptReaction", createReaction);
                if (!createResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to add your reaction.";
                }
            }

            // Redirect back to the page (could be prompt details or list)
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}

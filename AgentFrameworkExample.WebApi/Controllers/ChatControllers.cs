using Microsoft.AspNetCore.Mvc;

namespace AgentFrameworkExample.WebApi.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatControllers(IChatService chat) : ControllerBase
    {
        [HttpPost]
        public IActionResult PostMessage(MessageRequest request)
        {
            chat.SendMessageToLlm(request.Message);
            return Ok(new { status = "Message received and sent to LLM." });
        }
    }

    public record MessageRequest(string Message);
}


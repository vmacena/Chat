using Chat.Business;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController(MessagesBusiness messageBusiness) : ControllerBase
    {
        private readonly MessagesBusiness _messageBusiness = messageBusiness;

        [HttpGet("all")]
        public IActionResult GetAllMessages()
        {
            var messages = _messageBusiness.GetAllMessages().ToList();
            return Ok(messages);
        }
    }
}

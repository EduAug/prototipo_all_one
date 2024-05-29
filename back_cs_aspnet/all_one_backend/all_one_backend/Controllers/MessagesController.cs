using Microsoft.AspNetCore.Mvc;

namespace all_one_backend.Controllers
{
    [ApiController]
    [Route("secrecy/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ChatHub _hub;

        public MessagesController(ChatHub hub)
        {
            _hub = hub;
        }

        [HttpGet("{groupName}")]
        public IActionResult GetMessagesFrom(string groupName) 
        {
            try
            {
                var mssgs = _hub.getMessages(groupName);
                return Ok(mssgs);
            }
            catch (Exception ex) 
            {
                return StatusCode(500,"Something went wrong back here trying to get the messages" + ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MSyncBot.API.Types;

namespace MSyncBot.API.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private static Dictionary<string, List<Message>> messageStore = new();

        // POST api/messages/{socialNetwork}
        [HttpPost("{socialNetwork}")]
        public ActionResult<Message> SendMessage(string socialNetwork, Message message)
        {
            if (!messageStore.ContainsKey(socialNetwork))
            {
                messageStore[socialNetwork] = new List<Message>();
            }

            messageStore[socialNetwork].Add(message);
            return Ok(message);
        }

        // GET api/messages/{socialNetwork}
        [HttpGet("{socialNetwork}")]
        public ActionResult<List<Message>> GetMessages(string socialNetwork)
        {
            if (messageStore.TryGetValue(socialNetwork, out var value))
            {
                return Ok(value);
            }

            return NotFound();
        }
    }
}
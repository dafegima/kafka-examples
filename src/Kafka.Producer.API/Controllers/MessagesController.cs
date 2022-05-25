using Kafka.Producer.API.DTO;
using Kafka.Producer.API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kafka.Producer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ITopicRepository _topicRepository;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(ILogger<MessagesController> logger, ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] MessageRequest request)
        {
            _topicRepository.AddMessage(request);
            return Ok();
        }
    }
}
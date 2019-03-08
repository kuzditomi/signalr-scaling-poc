using Microsoft.AspNetCore.Mvc;

namespace SignalrScalingPoc.RabbitPublisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private RabbitPublisherService _publisherService { get; }

        public MessagesController(RabbitPublisherService publisher)
        {
            _publisherService = publisher;
        }

        [Route("")]
        [HttpPost]
        public ActionResult PushMessages(int count = 10)
        {
            _publisherService.PublishMessages(count);

            return Ok();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEventManagement.Repositories;
using System.Security.Claims;

namespace OnlineEventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventRegisterController : ControllerBase
    {
        private readonly IEventRegisterRepository eventRegisterRepository;
        private readonly IMapper mapper;

        public EventRegisterController(IEventRegisterRepository eventRegisterRepository, IMapper mapper)
        {
            this.eventRegisterRepository = eventRegisterRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        [Route("{eventId:Guid}")]
        [Authorize(Roles = "Admin,Participant")]
        //CREATE EVENT REGISTER
        public async Task<IActionResult> CreateEventRegister([FromRoute] Guid eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Please include User Id in Authentication.");

            Guid createdBy = Guid.Parse(userId);
            Guid? eventRegisterId = await eventRegisterRepository.AddEventRegisterAsync(eventId, createdBy);

            if(eventRegisterId == null)
                return Ok("Event has no more slots!!");

            return Ok("You are registered to event successfully!");
        }
    }
}

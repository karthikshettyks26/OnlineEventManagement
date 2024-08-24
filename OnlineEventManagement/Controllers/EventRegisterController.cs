using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEventManagement.Repositories;

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
        //CREATE EVENT REGISTER
        public async Task<IActionResult> CreateEventRegister([FromRoute] Guid eventId)
        {
            //ToDo - current user
            Guid createdBy = Guid.NewGuid();
            Guid? eventRegisterId = await eventRegisterRepository.AddEventRegisterAsync(eventId, createdBy);

            if(eventRegisterId == null)
                return Ok("Event has no more slots!!");

            return Ok("You are registered to event successfully!");
        }
    }
}

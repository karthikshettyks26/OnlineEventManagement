using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEventManagement.CustomActionFilter;
using OnlineEventManagement.Models.Domain;
using OnlineEventManagement.Models.DTO;
using OnlineEventManagement.Repositories;

namespace OnlineEventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository eventRepository;
        private readonly IMapper mapper;

        public EventController(IEventRepository eventRepository, IMapper mapper) {
            this.eventRepository = eventRepository;
            this.mapper = mapper;
        }

        //CREATE EVENT
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> AddEvent([FromBody] EventRequestDto eventRequestDto)
        {
            //Map Dto to domain model
            var @event = mapper.Map<Event>(eventRequestDto);

            //TODO:Add createdBy
            @event.CreatedBy = Guid.NewGuid();

            //Store it in DB
            Guid newEventId = await eventRepository.AddEventAsync(@event);

            if(newEventId == Guid.Empty)
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");

            return Ok(newEventId);
        }
    }
}

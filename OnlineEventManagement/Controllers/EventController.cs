using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEventManagement.CustomActionFilter;
using OnlineEventManagement.Models.Domain;
using OnlineEventManagement.Models.DTO;
using OnlineEventManagement.Repositories;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEvent([FromBody] AddEventRequestDto eventRequestDto)
        {
            //Map Dto to domain model
            var @event = mapper.Map<Event>(eventRequestDto);

            //TODO:Add createdBy
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            // Set the CreatedBy field to the logged-in user's ID
            @event.CreatedBy = Guid.Parse(userId);

            //Store it in DB
            Guid newEventId = await eventRepository.AddEventAsync(@event);

            if (newEventId == Guid.Empty)
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");

            return Ok(newEventId);
        }

        //GET ALL EVENTS
        [HttpGet]
        [Authorize(Roles = "Admin,Participant")]
        public async Task<IActionResult> GetAllEvents([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending)
        {
            //Get Events from DB
            var events = await eventRepository.GetAllEventsAsync(filterOn, filterQuery, sortBy, isAscending);

            return Ok(events);
        }

        //GET EVENT BY ID
        [HttpGet]
        [Authorize(Roles = "Admin,Participant")]
        [Route("{eventId:Guid}")]
        public async Task<IActionResult> GetEventById([FromRoute] Guid eventId)
        {
            //Get the event from DB
            var @event = await eventRepository.GetEventByIdAsync(eventId);

            if (@event == null)
                return NotFound();

            return Ok(@event);
        }

        //UPDATE
        [HttpPut]
        [ValidateModel]
        [Route("{eventId:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEvent([FromRoute] Guid eventId, [FromBody] UpdateEventRequestDto updateEventRequestDto)
        {
            //Map Dto to domain model
            var @event = mapper.Map<Event>(updateEventRequestDto);
            @event.Id = eventId;

            //Update into Db
            bool isUpdated = await eventRepository.UpdateEventAsync(@event, eventId);

            if (isUpdated)
                return Ok(@event);

            return NotFound();
        }

        //DELETE EVENT
        [HttpDelete]
        [Route("{eventId:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent([FromRoute] Guid eventId)
        {
           
            //Delete in Db
            bool isDeleted = await eventRepository.DeleteEventAsync(eventId);

            if (isDeleted)
                return Ok();

            return NotFound();
        }

    }
}

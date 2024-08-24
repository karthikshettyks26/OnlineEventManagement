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
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository feedbackRepository;
        private readonly IMapper mapper;

        public FeedbackController(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            this.feedbackRepository = feedbackRepository;
            this.mapper = mapper;
        }


        //CREATE FEEDBACK
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateFeedback([FromRoute] Guid eventId, [FromBody] AddFeedbackRequestDto addFeedbackRequestDto)
        {
            //Map Dto to domain model.
            var feedback = mapper.Map<Feedback>(addFeedbackRequestDto);
            //Todo:add user
            feedback.CreatedBy = Guid.NewGuid();
            feedback.EventId = eventId;

            //Add to db.
            Guid? feedbackId = await feedbackRepository.AddFeedbackAsync(feedback);

            if(feedbackId == null)
                return Ok("You have not registered to thid event / No such event exists!");

            return Ok(feedbackId);
        }

    
    }
}

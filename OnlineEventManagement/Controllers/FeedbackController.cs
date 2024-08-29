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
        [Route("{eventId:Guid}")]
        [Authorize(Roles = "Admin,Participant")]
        public async Task<IActionResult> CreateFeedback([FromRoute] Guid eventId, [FromBody] AddFeedbackRequestDto addFeedbackRequestDto)
        {
            //Map Dto to domain model.
            var feedback = mapper.Map<Feedback>(addFeedbackRequestDto);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Please include User Id in Authentication.");

            feedback.CreatedBy = Guid.Parse(userId);
            feedback.EventId = eventId;

            //Add to db.
            Guid? feedbackId = await feedbackRepository.AddFeedbackAsync(feedback);

            return Ok("Your feedback added successfully!");
        }

    
    }
}

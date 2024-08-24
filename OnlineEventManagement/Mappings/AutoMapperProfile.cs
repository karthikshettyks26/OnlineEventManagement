using AutoMapper;
using OnlineEventManagement.Models.Domain;
using OnlineEventManagement.Models.DTO;

namespace OnlineEventManagement.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Event, AddEventRequestDto>().ReverseMap();
            CreateMap<Event,UpdateEventRequestDto>().ReverseMap();
            CreateMap<Feedback,AddFeedbackRequestDto>().ReverseMap();
        }
    }
}

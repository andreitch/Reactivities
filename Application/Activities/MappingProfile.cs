using AutoMapper;
using Domain;

namespace Application.Activities
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
        CreateMap<Activity, ActivityDto>();
        CreateMap<UserActivity, AttendeeDto>();
    }
  }
}
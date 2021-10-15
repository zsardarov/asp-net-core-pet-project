using System.Linq;
using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Activity, Activity>();
            CreateMap<Activity, ActivityDto>()
                .ForMember(destination => destination.HostUsername,
                    opt =>
                    {
                        opt.MapFrom(source =>
                            source.Attendees.FirstOrDefault(attendee => attendee.IsHost).User.DisplayName);
                    });
            CreateMap<ActivityAttendee, AttendeeDto>()
                .ForMember(destination => destination.DisplayName,
                    opt => opt.MapFrom(source => source.User.DisplayName))
                .ForMember(destination => destination.Username, opt => opt.MapFrom(source => source.User.UserName))
                .ForMember(destination => destination.Bio, opt => opt.MapFrom(source => source.User.Bio))
                .ForMember(destination => destination.Image,
                    opt => opt.MapFrom(source => source.User.Photos.FirstOrDefault(photo => photo.IsMain).Url));

            CreateMap<User, Profiles.Profile>()
                .ForMember(destination => destination.Image,
                    opt => opt.MapFrom(source => source.Photos.FirstOrDefault(photo => photo.IsMain).Url));
        }
    }
}
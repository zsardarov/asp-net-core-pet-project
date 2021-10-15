using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.UseCases
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }
        
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(IUserAccessor userAccessor, DataContext context)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            
            
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.Include(activity => activity.Attendees)
                    .ThenInclude(attendee => attendee.User)
                    .FirstOrDefaultAsync(activity => activity.Id == request.Id);

                if (activity == null)
                {
                    return null;
                }

                var user = await _context.Users.FirstOrDefaultAsync(user => user.UserName == _userAccessor.GetUsername());

                if (user == null)
                {
                    return null;
                }

                var hostUserId = activity.Attendees.FirstOrDefault(attendee => attendee.IsHost)?.UserId;

                var userAttendance = activity.Attendees.FirstOrDefault(attendee => attendee.UserId == user.Id);

                if (userAttendance != null && hostUserId == user.Id)
                {
                    activity.IsCanceled = !activity.IsCanceled;
                }

                if (userAttendance != null && hostUserId != user.Id)
                {
                    activity.Attendees.Remove(userAttendance);
                }

                if (userAttendance == null)
                {
                    activity.Attendees.Add(new ActivityAttendee
                    {
                        Activity = activity,
                        User = user,
                        IsHost = false
                    });
                }

                var result = await _context.SaveChangesAsync() > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed updating attendees");
            }
        }
    }
}
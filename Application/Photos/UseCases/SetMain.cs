using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos.UseCases
{
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .Include(user => user.Photos)
                    .FirstOrDefaultAsync(user => user.UserName == _userAccessor.GetUsername());
                
                if (user == null)
                {
                    return null;
                }

                var requestedPhoto = user.Photos.FirstOrDefault(photo => photo.Id == request.Id);

                if (requestedPhoto == null)
                {
                    return null;
                }

                var currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);
                
                if (currentMain == null)
                {
                    return null;
                }

                if (currentMain.Id == requestedPhoto.Id)
                {
                    return Result<Unit>.Failure("Already set as main");
                }

                currentMain.IsMain = false;
                requestedPhoto.IsMain = true;

                var result = await _context.SaveChangesAsync() > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to set main photo");
            }
        }
    }
}
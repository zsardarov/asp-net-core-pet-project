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
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IPhotoAccessor _photoAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
                _photoAccessor = photoAccessor;
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

                var photo = user.Photos.FirstOrDefault(photo => photo.Id == request.Id);

                if (photo == null)
                {
                    return null;
                }

                if (photo.IsMain)
                {
                    return Result<Unit>.Failure("You can't delete main photo");
                }

                var result = _photoAccessor.DeletePhoto(request.Id);

                if (result == null)
                {
                    return Result<Unit>.Failure("Failed to delete photo from storage");
                }

                user.Photos.Remove(photo);

                var success = await _context.SaveChangesAsync() > 0;

                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to delete photo");
            }
        }
    }
}
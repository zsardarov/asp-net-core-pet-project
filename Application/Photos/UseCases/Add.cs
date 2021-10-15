using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos.UseCases
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>
        {
            public IFormFile file { get; set; }
        }
        
        public class Handler : IRequestHandler<Command, Result<Photo>>
        {
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _context = context;
                _photoAccessor = photoAccessor;
                _userAccessor = userAccessor;
            }
            
            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .Include(user => user.Photos)
                    .FirstOrDefaultAsync(user => user.UserName == _userAccessor.GetUsername());

                if (user == null)
                {
                    return null;
                }

                var uploadResult = await _photoAccessor.AddPhoto(request.file);

                var photo = new Photo
                {
                    Url = uploadResult.Url,
                    Id = uploadResult.PublicId
                };

                if (!user.Photos.Any(p => p.IsMain))
                {
                    photo.IsMain = true;
                }
                user.Photos.Add(photo);
                
                var result =  await _context.SaveChangesAsync() > 0;

                return result ? Result<Photo>.Success(photo) : Result<Photo>.Failure("Failed to add photo");
            }
        }
    }
}
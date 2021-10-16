using System.Threading.Tasks;
using Application.Followers.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FollowController : BaseApiController
    {
        [HttpPost("{username}")]
        public async Task<IActionResult> ToggleFollowing(string username)
        {
            return HandleResult(await Mediator.Send(new FollowToggle.Command {TargetUsername = username}));
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetFollowings(string username, string predicate)
        {
            return HandleResult(await Mediator.Send(new List.Query {Predicate = predicate, Username = username}));
        }
    }
}
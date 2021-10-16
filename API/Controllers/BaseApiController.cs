using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ActionResult HandleResult<T>(Result<T> result, int expectedStatusCode = 200)
        {
            if (result == null)
            {
                return NotFound();
            }

            if (result.IsSuccess && result.Value == null)
            {
                return NotFound();
            }

            if (result.IsSuccess && result.Value != null)
            {
                return GetResponseByStatusCode(expectedStatusCode, result.Value);
            }

            if (!result.IsSuccess && result.Error != null)
            {
                return BadRequest(result);
            }

            return BadRequest();
        }

        private ActionResult GetResponseByStatusCode<T>(int code, T value)
        {
            switch (code)
            {
                case 201:
                case 204:
                    return StatusCode(code);
                default:
                    return Ok(value);
            }
        }
    }
}
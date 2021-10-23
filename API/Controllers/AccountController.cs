using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include(user => user.Photos)
                .FirstOrDefaultAsync(user => user.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result.Succeeded)
            {
                await SetRefreshToken(user);
                return CreateUserDto(user);
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("refreshTokens")]
        public async Task<IActionResult> RefreshTokens()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            var userId = _tokenService.Validate(refreshToken);

            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.Users.Include(user => user.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);
            var refreshTokenEntity = user.RefreshTokens
                .FirstOrDefault(t => t.Token == refreshToken);

            if (refreshTokenEntity == null)
            {
                return Unauthorized();
            }

            user.RefreshTokens.Remove(refreshTokenEntity);

            await SetRefreshToken(user);
            return Ok(new {Token = _tokenService.CreateAccessToken(user)});
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(user => user.Email == registerDto.Email))
            {
                return BadRequest("Email already taken");
            }

            if (await _userManager.Users.AnyAsync(user => user.UserName == registerDto.Username))
            {
                return BadRequest("Username already taken");
            }

            var user = new User
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return CreateUserDto(user);
            }

            return BadRequest("Problem during registering user");
        }

        [Authorize]
        [HttpGet]
        public async Task<UserDto> GetCurrentUser()
        {
            var user = await _userManager.Users
                .Include(user => user.Photos)
                .FirstOrDefaultAsync(user => user.Email == User.FindFirstValue(ClaimTypes.Email));

            return CreateUserDto(user);
        }

        private async Task SetRefreshToken(User user)
        {
            var refreshToken = _tokenService.CreateRefreshToken(user);
            
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(10),
                HttpOnly = true
            };
            
            Response.Cookies.Append("RefreshToken", refreshToken.Token, cookieOptions);
        }
        
        private UserDto CreateUserDto(User user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Image = user.Photos?.FirstOrDefault(photo => photo.IsMain)?.Url,
                Token = _tokenService.CreateAccessToken(user)
            };
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PokeDex.API.DTOs;
using PokeDex.API.Services;
using PokeDex.Data;

namespace PokeDex.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtTokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<AppUser> userManager,
            IJwtTokenService tokenService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Create an account and sign in
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(" ", result.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("Registered new user {Email}", request.Email);
            return Ok(_tokenService.CreateToken(user));
        }

        /// <summary>
        /// Sign in with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(_tokenService.CreateToken(user));
        }
    }
}

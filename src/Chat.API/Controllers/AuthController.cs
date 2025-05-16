using Chat.API.Services;
using Chat.Business.Services;
using Chat.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserService userService, JwtService jwtService) : ControllerBase
{
    private readonly UserService _userService = userService;
    private readonly JwtService _jwtService = jwtService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        if (await _userService.EmailExistsAsync(dto.Email))
            return BadRequest("Email already exists.");

        var user = await _userService.RegisterAsync(dto);
        return Ok(
            new
            {
                user.Id,
                user.Name,
                user.Email,
            }
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _userService.AuthenticateAsync(dto);
        if (user == null)
            return Unauthorized("User not found or password incorrect.");

        var token = _jwtService.GenerateToken(user);

        return Ok(new { token });
    }
}

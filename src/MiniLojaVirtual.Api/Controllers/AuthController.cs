using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using MiniLojaVirtual.Api.Controllers.Base;
using MiniLojaVirtual.Api.Models.Auth;
using MiniLojaVirtual.Api.Services;
using MiniLojaVirtual.Infrastructure.Entities;

using System.Net;

namespace MiniLojaVirtual.Api.Controllers;

[AllowAnonymous]
public class AuthController : BaseApiController
{
	private readonly UserManager<UserEntity> _userManager;
	private readonly SignInManager<UserEntity> _signInManager;
	private readonly TokenService _tokenService;

	public AuthController(
		ILogger<AuthController> logger,
		UserManager<UserEntity> userManager,
		SignInManager<UserEntity> signInManager,
		TokenService tokenService)
		: base(logger)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_tokenService = tokenService;
	}

	[HttpPost("login")]
	[ProducesResponseType(typeof(TokenResponseDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var user = await _userManager.FindByEmailAsync(loginDto.Email);
		if (user == null)
			return BadRequest(new { message = "Usuário ou senha inválidos" });

		var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
		if (!result.Succeeded)
			return BadRequest(new { message = "Usuário ou senha inválidos" });

		var token = await _tokenService.GenerateToken(user);
		return Ok(token);
	}

	[HttpPost("register")]
	[ProducesResponseType(typeof(TokenResponseDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
		if (userExists != null)
			return BadRequest(new { message = "Email já está em uso" });

		var user = new UserEntity
		{
			UserName = registerDto.Email,
			Email = registerDto.Email,
			Name = registerDto.Name,
			CreatedAt = DateTime.UtcNow,
			EmailConfirmed = true
		};

		var result = await _userManager.CreateAsync(user, registerDto.Password);
		if (!result.Succeeded)
			return BadRequest(new { message = result.Errors.First().Description });

		var token = await _tokenService.GenerateToken(user);
		return Ok(token);
	}
}
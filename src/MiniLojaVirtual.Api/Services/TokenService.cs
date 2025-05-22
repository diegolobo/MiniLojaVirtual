using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using MiniLojaVirtual.Api.Models.Auth;
using MiniLojaVirtual.Infrastructure.Entities;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniLojaVirtual.Api.Services;

public class TokenService
{
	private readonly UserManager<UserEntity> _userManager;
	private readonly IConfiguration _configuration;

	public TokenService(
		UserManager<UserEntity> userManager,
		IConfiguration configuration)
	{
		_userManager = userManager;
		_configuration = configuration;
	}

	public async Task<TokenResponseDto> GenerateToken(UserEntity user)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Name, user.UserName),
			new(ClaimTypes.Email, user.Email),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var roles = await _userManager.GetRolesAsync(user);
		foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var expiration = DateTime.UtcNow.AddHours(double.Parse(_configuration["JwtSettings:ExpirationHours"]));

		var token = new JwtSecurityToken(
			_configuration["JwtSettings:Issuer"],
			_configuration["JwtSettings:Audience"],
			claims,
			expires: expiration,
			signingCredentials: creds);

		return new TokenResponseDto
		{
			Token = new JwtSecurityTokenHandler().WriteToken(token),
			Expiration = expiration
		};
	}
}
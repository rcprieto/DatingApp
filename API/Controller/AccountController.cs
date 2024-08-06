using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

[AllowAnonymous]
public class AccountController : BaseApiController
{
	private readonly DataContext _context;
	private readonly ITokenService _tokenService;
	private readonly IMapper _mapper;

	public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
	{
		_context = context;
		_tokenService = tokenService;
		_mapper = mapper;
	}

	[HttpPost("register")] //POST: api/account/register
	public async Task<ActionResult<UserDto>> Register(RegisterDto model)
	{
		using var hmac = new HMACSHA512();

		if (await UserExists(model.UserName))
		{
			return BadRequest("User Name Já existe");

		}

		var user = _mapper.Map<AppUser>(model);

		user.UserName = model.UserName.Trim();
		user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
		user.PasswordSalt = hmac.Key;

		_context.AppUsers.Add(user);
		await _context.SaveChangesAsync();

		return new UserDto
		{
			Username = user.UserName,
			Token = _tokenService.CreateToken(user),
			KnownAs = user.KnownAs

		};

	}


	[HttpPost("login")]
	public async Task<ActionResult<UserDto>> Login(LoginDto model)
	{
		var user = await _context.AppUsers
			.Include(c => c.Photos)
			.SingleOrDefaultAsync(m => m.UserName == model.UserName);

		if (user == null) return Unauthorized();


		using var hmac = new HMACSHA512(user.PasswordSalt);
		var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
		for (int i = 0; i < hash.Length; i++)
		{
			if (hash[i] != user.PasswordHash[i])
				return Unauthorized("Login Inválido");
		}



		return new UserDto
		{
			Username = user.UserName,
			Token = _tokenService.CreateToken(user),
			PhotoUrl = user.Photos.FirstOrDefault(c => c.IsMain)?.Url,
			KnownAs = user.KnownAs,
			Gender = user.Gender

		};

	}


	private async Task<bool> UserExists(string username)
	{
		return await _context.AppUsers.AnyAsync(c => c.UserName.ToUpper().Equals(username.ToUpper().Trim()));
	}


}

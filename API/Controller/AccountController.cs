using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

[AllowAnonymous]
public class AccountController : BaseApiController
{
	private readonly UserManager<AppUser> _userMananger;
	private readonly DataContext _context;
	private readonly ITokenService _tokenService;
	private readonly IMapper _mapper;

	public AccountController(UserManager<AppUser> userMananger, DataContext context, ITokenService tokenService, IMapper mapper)
	{
		_userMananger = userMananger;
		_context = context;
		_tokenService = tokenService;
		_mapper = mapper;
	}

	[HttpPost("register")] //POST: api/account/register
	public async Task<ActionResult<UserDto>> Register(RegisterDto model)
	{
		var user = _mapper.Map<AppUser>(model);

		user.UserName = model.UserName.Trim();

		var result = await _userMananger.CreateAsync(user, model.Password);

		if (!result.Succeeded) return BadRequest(result.Errors);

		var rolesResult = await _userMananger.AddToRoleAsync(user, "Member");

		if (!rolesResult.Succeeded) return BadRequest(result.Errors);

		return new UserDto
		{
			Username = user.UserName,
			Token = await _tokenService.CreateToken(user),
			KnownAs = user.KnownAs

		};

	}


	[HttpPost("login")]
	public async Task<ActionResult<UserDto>> Login(LoginDto model)
	{
		var user = await _userMananger.Users
			.Include(c => c.Photos)
			.SingleOrDefaultAsync(m => m.UserName == model.UserName);

		if (user == null) return Unauthorized();

		var result = await _userMananger.CheckPasswordAsync(user, model.Password);

		if (!result) return Unauthorized();

		return new UserDto
		{
			Username = user.UserName,
			Token = await _tokenService.CreateToken(user),
			PhotoUrl = user.Photos.FirstOrDefault(c => c.IsMain)?.Url,
			KnownAs = user.KnownAs,
			Gender = user.Gender

		};

	}


	private async Task<bool> UserExists(string username)
	{
		return await _userMananger.Users.AnyAsync(c => c.UserName.ToUpper().Equals(username.ToUpper().Trim()));
	}


}

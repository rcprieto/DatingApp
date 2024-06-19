using System.Security.Claims;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controller;

[Authorize]
public class UsersController : BaseApiController
{
	private readonly IAppUserRepository _appUserRepository;
	private readonly IMapper _mapper;

	public UsersController(IAppUserRepository appUserRepository, IMapper mapper)
	{
		_appUserRepository = appUserRepository;
		_mapper = mapper;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
	{
		var users = await _appUserRepository.GetUsersAsync();
		var members = _mapper.Map<IEnumerable<MemberDto>>(users);
		return Ok(members);
	}

	[HttpGet("getbyusername")]
	public async Task<ActionResult<MemberDto>> GetUser([FromQuery] string username)
	{
		//return _mapper.Map<MemberDto>(await _appUserRepository.GetUserByUsernameAsync(username));
		return _mapper.Map<MemberDto>(await _appUserRepository.GetMemberAsync(username));
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<AppUser>> GetUser(int id)
	{
		return await _appUserRepository.GetUserById(id);
	}

	[HttpPut]
	public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
	{
		var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		var user = await _appUserRepository.GetUserByUsernameAsync(username);
		if (user == null) return NotFound();

		_mapper.Map(memberUpdateDto, user);

		if (await _appUserRepository.SaveAllAsync()) return NoContent();

		return BadRequest("Erro ao atualizar o usuário");


	}
}

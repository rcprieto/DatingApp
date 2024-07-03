using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller;

[Authorize]
public class UsersController : BaseApiController
{
	private readonly IAppUserRepository _appUserRepository;
	private readonly IMapper _mapper;
	private readonly IPhotoService _photoService;

	public UsersController(IAppUserRepository appUserRepository, IMapper mapper, IPhotoService photoService)
	{
		_appUserRepository = appUserRepository;
		_mapper = mapper;
		_photoService = photoService;
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
		var user = await _appUserRepository.GetUserByUsernameAsync(User.GetUserName());
		if (user == null) return NotFound();

		_mapper.Map(memberUpdateDto, user);

		if (await _appUserRepository.SaveAllAsync()) return NoContent();

		return BadRequest("Erro ao atualizar o usuário");


	}

	[HttpPost("add-photo")]
	public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
	{

		var user = await _appUserRepository.GetUserByUsernameAsync(User.GetUserName());
		if (user == null) return NotFound();

		var result = await _photoService.AddPhotoAsync(file);
		if (result.Error != null) return BadRequest(result.Error.Message);

		var photo = new Photo
		{
			Url = result.SecureUrl.AbsoluteUri,
			PublicId = result.PublicId
		};

		if (user.Photos.Count == 0) photo.IsMain = true;

		user.Photos.Add(photo);

		if (await _appUserRepository.SaveAllAsync())
		{
			return CreatedAtAction(
				nameof(GetUser),
			new { username = user.UserName },
			 _mapper.Map<PhotoDto>(photo));
			//return _mapper.Map<PhotoDto>(photo);
		}

		return BadRequest("Erro ao subir a foto");

	}

	[HttpPut("set-main-photo/{photoId}")]
	public async Task<ActionResult> SetMainPhoto(int photoId)
	{
		var user = await _appUserRepository.GetUserByUsernameAsync(User.GetUserName());
		if (user == null) return NotFound();
		var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
		if (photo == null) return NotFound();
		if (photo.IsMain) return BadRequest("Already main");

		var currentMain = user.Photos.FirstOrDefault(c => c.IsMain);

		if (currentMain != null) currentMain.IsMain = false;

		photo.IsMain = true;
		if (await _appUserRepository.SaveAllAsync()) return NoContent();

		return BadRequest("Problem setting to main");

	}

	[HttpDelete("delete-photo/{photoId}")]
	public async Task<ActionResult> DeletePhoto(int photoId)
	{
		var user = await _appUserRepository.GetUserByUsernameAsync(User.GetUserName());
		var photos = user.Photos.FirstOrDefault(c => c.Id == photoId);
		if (photos == null) return NotFound();
		if (photos.IsMain) return BadRequest("Não pode excluir a foto principal");

		if (photos.PublicId != null)
		{
			var result = await _photoService.DeletePhotoAsync(photos.PublicId);
			if (result.Error != null) return BadRequest(result.Error.Message);
		}

		user.Photos.Remove(photos);
		if (await _appUserRepository.SaveAllAsync()) return Ok();

		return BadRequest("Ocorreu um erro ao excluir");
	}

	[HttpGet("teste")]
	public async Task<ActionResult> Teste()
	{


		return Ok("Ocorreu um erro ao excluir");
	}

}

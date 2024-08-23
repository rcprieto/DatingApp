using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class BuggyController : BaseApiController
{
	private readonly DataContext _context;

	public BuggyController(DataContext context)
	{
		_context = context;
	}

	[Authorize]
	[HttpGet("auth")]
	public ActionResult<string> GetSecret()
	{
		return "secret text";
	}

	[HttpGet("not-found")]
	public ActionResult<AppUser> GetNotFound()
	{
		var item = _context.Users.Find(-1);
		if (item == null) return NotFound();

		return item;
	}

	[HttpGet("server-error")]
	public ActionResult<string> GetServerError()
	{
		var item = _context.Users.Find(-1);
		var retorno = item.ToString();
		return retorno;
	}

	[HttpGet("bad-request")]
	public ActionResult<string> GetBadRequest()
	{
		return BadRequest("Requisição Inválida");
	}


}

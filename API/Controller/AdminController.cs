
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controller
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("user-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
            .OrderBy(c => c.UserName)
            .Select(c => new
            {
                c.Id,
                Username = c.UserName,
                Roles = c.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Apenas para admin e moderadores");
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {

            if (string.IsNullOrEmpty(roles)) return BadRequest("Sem Roles");

            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            //Adiciona as roles que ele não tem ainda
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Erro ao incluir Roles");

            //Remove outras que não estão na lista
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Erro ao excluir Roles");

            return Ok(await _userManager.GetRolesAsync(user));



        }



    }
}

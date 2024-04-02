using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class AppUser
{
	[Key]
	public int Id { get; set; }

	[StringLength(300)]
	public string UserName { get; set; }


}

using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		var resultContext = await next();
		if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

		var userId = Convert.ToInt32(resultContext.HttpContext.User.GetUserId());
		var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IAppUserRepository>();
		var user = await repo.GetUserById(userId);
		user.LastActive = DateTime.UtcNow;
		await repo.SaveAllAsync();

	}
}

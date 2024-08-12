using API.Data;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class ApplicationServiceExtensions
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
	{
		services.AddDbContext<DataContext>(opt =>
		{
			opt.UseSqlite(config.GetConnectionString("DefaultConnection"));

		});

		services.AddCors();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<IAppUserRepository, AppUserRepository>();
		services.AddScoped<IPhotoService, PhotoService>();
		services.AddScoped<ILikesRepository, LikeRepository>();
		services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
		services.AddScoped<LogUserActivity>();

		return services;
	}

}

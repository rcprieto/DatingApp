
using API;
using API.Data;
using API.Entities;
using API.SignaIR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);



var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseCors(x =>
	x.AllowAnyHeader()
	.AllowAnyMethod()
	.AllowCredentials() //SignaIR precisa
	.WithOrigins("https://localhost:4200"));

app.UseAuthentication(); //Vc tem um token valido
app.UseAuthorization(); //Vc tem um token mas o que vc pode fazer


app.MapControllers();
//SignalIR
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
	var context = services.GetRequiredService<DataContext>();
	var userManager = services.GetRequiredService<UserManager<AppUser>>();
	var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
	await context.Database.MigrateAsync();
	await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]"); //Para sqllite
																			//await context.Database.ExecuteSqlRawAsync("TRUNCATE FROM TABLE [Connections]");
	await Seed.SeedUser(userManager, roleManager);

}
catch (Exception ex)
{
	var logger = services.GetService<ILogger<Program>>();
	logger.LogError(ex, "Erro durante o migration");

}


app.Run();

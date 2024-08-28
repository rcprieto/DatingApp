using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
	public DataContext(DbContextOptions options) : base(options)
	{
		//dotnet ef migrations add NomeMigracao
	}

	public DbSet<Photo> Photos { get; set; }
	public DbSet<UserLike> Likes { get; set; }
	public DbSet<Message> Messages { get; set; }
	public DbSet<Group> Groups { get; set; }
	public DbSet<Connection> Connections { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<AppUser>()
		.HasMany(c => c.UserRoles)
		.WithOne(c => c.User)
		.HasForeignKey(c => c.UserId)
		.IsRequired();

		builder.Entity<AppRole>()
		.HasMany(c => c.UserRoles)
		.WithOne(c => c.Role)
		.HasForeignKey(c => c.RoleId)
		.IsRequired();


		builder.Entity<UserLike>()
		.HasKey(c => new { c.SourceUserId, c.TargetUserId });

		builder.Entity<UserLike>()
		.HasOne(c => c.SourceUser)
		.WithMany(c => c.LikedUsers)
		.HasForeignKey(c => c.SourceUserId)
		.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<UserLike>()
		.HasOne(c => c.TargetUser)
		.WithMany(c => c.LikedByUsers)
		.HasForeignKey(c => c.TargetUserId)
		.OnDelete(DeleteBehavior.Cascade); //No sql server um precisa ser NoAction

		builder.Entity<Message>()
		.HasOne(c => c.Recipient)
		.WithMany(c => c.MessagesReceived)
		.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<Message>()
		.HasOne(c => c.Sender)
		.WithMany(c => c.MessagesSent)
		.OnDelete(DeleteBehavior.Restrict);




	}
}

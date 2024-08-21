using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions options) : base(options)
	{
		//dotnet ef migrations add NomeMigracao
	}


	public DbSet<AppUser> AppUsers { get; set; }
	public DbSet<Photo> Photos { get; set; }
	public DbSet<UserLike> Likes { get; set; }
	public DbSet<Message> Messages { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

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

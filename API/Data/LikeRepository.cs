using System;
using API.Controller;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikeRepository : ILikesRepository
{
	private readonly DataContext _context;

	public LikeRepository(DataContext context)
	{
		_context = context;
	}
	public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
	{
		return await _context.Likes.FindAsync(sourceUserId, targetUserId);
	}

	public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
	{
		var users = _context.AppUsers.OrderBy(c => c.UserName).AsQueryable();
		var likes = _context.Likes.AsQueryable();

		if (likeParams.Predicate == "liked")
		{
			likes = likes.Where(c => c.SourceUserId == likeParams.UserId);
			users = likes.Select(c => c.TargetUser);
		}
		if (likeParams.Predicate == "likedBy")
		{
			likes = likes.Where(c => c.TargetUserId == likeParams.UserId);
			users = likes.Select(c => c.SourceUser);
		}

		var likedUsers = users.Select(c => new LikeDto
		{
			UserName = c.UserName,
			Age = c.DateOfBirth.CalculateAge(),
			KnownAs = c.KnownAs,
			City = c.City,
			PhotoUrl = c.Photos.FirstOrDefault(x => x.IsMain).Url,
			Id = c.Id

		});

		return await PagedList<LikeDto>.CreateAsync(likedUsers, likeParams.PageNumber, likeParams.PageSize);



	}

	public async Task<AppUser> GetUserWithLikes(int userId)
	{
		return await _context.AppUsers
		.Include(c => c.LikedUsers)
		.FirstOrDefaultAsync(x => x.Id == userId);

	}
}

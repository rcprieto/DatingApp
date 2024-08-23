using API.Data;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AppUserRepository : IAppUserRepository
{
	private readonly DataContext _context;
	private readonly IMapper _mapper;

	public AppUserRepository(DataContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<MemberDto> GetMemberAsync(string username)
	{
		return await _context.Users
		.Where(c => c.UserName == username)
		.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
		.SingleOrDefaultAsync();
	}

	public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
	{
		var query = _context.Users.AsQueryable();
		query = query.Where(c => c.UserName != userParams.CurrentUserName && c.Gender == userParams.Gender);

		var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
		var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

		query = query.Where(c => c.DateOfBirth >= minDob && c.DateOfBirth <= maxDob);

		query = userParams.OrderBy switch
		{
			"created" => query.OrderByDescending(c => c.Created),
			_ => query.OrderByDescending(c => c.LastActive)
		};

		return await PagedList<MemberDto>
		.CreateAsync(query.AsNoTracking()
					.ProjectTo<MemberDto>(_mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);

	}

	public async Task<AppUser> GetUserById(int id)
	{
		return await _context.Users.FindAsync(id);
	}

	public async Task<AppUser> GetUserByUsernameAsync(string username)
	{
		return await _context.Users
		.Include(c => c.Photos)
		.SingleOrDefaultAsync(c => c.UserName == username);
	}

	public async Task<IEnumerable<AppUser>> GetUsersAsync()
	{
		return await _context.Users
		.Include(c => c.Photos)
		.ToListAsync();
	}


	public async Task<bool> SaveAllAsync()
	{
		return await _context.SaveChangesAsync() > 0;
	}

	public void Update(AppUser user)
	{
		_context.Entry(user).State = EntityState.Modified;

	}
}

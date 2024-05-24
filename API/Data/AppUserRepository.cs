using API.Data;
using API.Entities;
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
		return await _context.AppUsers
		.Where(c => c.UserName == username)
		.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
		.SingleOrDefaultAsync();
	}

	public async Task<IEnumerable<MemberDto>> GetMembersAsync()
	{
		return await _context.AppUsers
		.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).ToListAsync();

	}

	public async Task<AppUser> GetUserById(int id)
	{
		return await _context.AppUsers.FindAsync(id);
	}

	public async Task<AppUser> GetUserByUsernameAsync(string username)
	{
		return await _context.AppUsers
		.Include(c => c.Photos)
		.SingleOrDefaultAsync(c => c.UserName == username);
	}

	public async Task<IEnumerable<AppUser>> GetUsersAsync()
	{
		return await _context.AppUsers
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

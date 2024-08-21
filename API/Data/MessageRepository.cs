using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository : IMessageRepository
{
	private readonly DataContext _context;
	private readonly IMapper _mapper;

	public MessageRepository(DataContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}
	public void AddMessage(Message message)
	{
		_context.Messages.Add(message);
	}

	public void DeleteMessage(Message message)
	{
		_context.Messages.Remove(message);
	}

	public async Task<Message> GetMessage(int id)
	{
		return await _context.Messages.FindAsync(id);
	}

	public async Task<PagedList<MessageDto>> GetMessagessForUser(MessageParams messageParams)
	{
		var query = _context.Messages
		.OrderByDescending(c => c.MessageSent)
		.AsQueryable();

		query = messageParams.Container switch
		{
			"Inbox" => query.Where(c => c.RecipientUsername == messageParams.Username && !c.RecipientDeleted),
			"Outbox" => query.Where(c => c.SenderUsername == messageParams.Username && !c.RecipientDeleted),
			_ => query.Where(c => c.RecipientUsername == messageParams.Username && !c.DateRead.HasValue && !c.RecipientDeleted)
		};

		var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

		return await PagedList<MessageDto>
				.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

	}

	public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
	{
		var messages = await _context.Messages
					.Include(c => c.Sender).ThenInclude(c => c.Photos)
					.Include(c => c.Recipient).ThenInclude(c => c.Photos)
					.Where(c => c.RecipientUsername == currentUserName && !c.RecipientDeleted
					&& c.SenderUsername == recipientUserName
					|| c.RecipientUsername == recipientUserName && !c.SenderDeletete
					&& c.SenderUsername == currentUserName)
					.OrderBy(c => c.MessageSent)
					.ToListAsync();

		var unreadMessagens = messages.Where(c => c.DateRead == null && c.RecipientUsername == currentUserName).ToList();

		if (unreadMessagens.Any())
		{
			foreach (var message in unreadMessagens)
			{
				message.DateRead = DateTime.UtcNow;
			}
			await _context.SaveChangesAsync();
		}

		return _mapper.Map<IEnumerable<MessageDto>>(messages);


	}

	public async Task<bool> SaveAllAsync()
	{
		return await _context.SaveChangesAsync() > 0;
	}
}

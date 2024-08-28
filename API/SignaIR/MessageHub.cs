
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;

namespace API.SignaIR;


[Authorize]
public class MessageHub(IMessageRepository messageRepository, IAppUserRepository appUserRepository, IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
{
	private readonly IMessageRepository _messageRepository = messageRepository;
	private readonly IAppUserRepository _appUserRepository = appUserRepository;
	private readonly IMapper _mapper = mapper;
	private readonly IHubContext<PresenceHub> _presenceHub = presenceHub;

	public override async Task OnConnectedAsync()
	{
		var httpContext = Context.GetHttpContext();
		var otherUser = httpContext.Request.Query["user"];
		var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
		var group = await AddToGroup(groupName);

		await Clients.Group(groupName).SendAsync("UpdateGroup", group);

		var messages = await _messageRepository
		.GetMessageThread(Context.User.GetUserName(), otherUser);

		await Clients.Caller.SendAsync("ReceivedMessageThread", messages);

	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
		var group = await RemoveFromMessageGroup();
		await Clients.Group(group.Name).SendAsync("UpdateGroup", group);
		await base.OnDisconnectedAsync(exception);
	}

	public async Task SendMessage(CreateMessageDto createMessageDto)
	{
		var username = Context.User.GetUserName();

		if (username.ToLower() == createMessageDto.RecipientUsername.ToLower())
			throw new HubException("Não pode mandar mensagens pra você");

		var sender = await _appUserRepository.GetUserByUsernameAsync(username);
		var recipient = await _appUserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

		var message = new Message
		{
			Sender = sender,
			Recipient = recipient,
			SenderUsername = sender.UserName,
			RecipientUsername = recipient.UserName,
			Content = createMessageDto.Content
		};

		var groupName = GetGroupName(sender.UserName, recipient.UserName);
		var group = await _messageRepository.GetMessageGroup(groupName);
		if (group.Connections.Any(x => x.Username == recipient.UserName))
		{
			message.DateRead = DateTime.UtcNow;
		}
		else
		{
			var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
			if (connections != null)
			{
				await _presenceHub.Clients.Clients(connections)
				.SendAsync("NewMessageReceived", new { username = sender.UserName, knownAs = sender.KnownAs });
			}
		}

		_messageRepository.AddMessage(message);
		if (await _messageRepository.SaveAllAsync())
			await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

	}

	private string GetGroupName(string caller, string other)
	{
		//ordena os nomes 
		var stringCompare = string.CompareOrdinal(caller, other) < 0;
		return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
	}

	private async Task<Group> AddToGroup(string groupName)
	{
		var group = await _messageRepository.GetMessageGroup(groupName);
		var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());
		if (group == null)
		{
			group = new Group(groupName);
			_messageRepository.AddGroup(group);
		}

		group.Connections.Add(connection);
		if (await _messageRepository.SaveAllAsync()) return group;

		throw new HubException("Erro ao adicionar o grupo");

	}

	private async Task<Group> RemoveFromMessageGroup()
	{
		var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
		var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
		_messageRepository.RemoveConnection(connection);
		if (await _messageRepository.SaveAllAsync()) return group;

		throw new HubException("Erro ao remover grupo");

	}
}

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignaIR;

//SignalIR
[Authorize]
public class PresenceHub : Hub
{
	private readonly PresenceTracker _tracker;

	public PresenceHub(PresenceTracker tracker)
	{
		_tracker = tracker;
	}
	//npm install @microsoft/signalr na client do angular
	public override async Task OnConnectedAsync()
	{
		var isOnline = await _tracker.UserConnectd(Context.User.GetUserName(), Context.ConnectionId);
		if (isOnline)
			await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

		var currentUsers = await _tracker.GetOnlineUsers();
		await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);



	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
		var isOffLine = await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
		if (isOffLine)
			await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());


		await base.OnDisconnectedAsync(exception);
	}

}

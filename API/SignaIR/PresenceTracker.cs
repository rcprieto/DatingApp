using System;
using System.Collections.Generic;

namespace API.SignaIR;

public class PresenceTracker
{
	//O certo é ser no banco de dados para bases grandes
	private static readonly Dictionary<string, List<string>> OnLineUsers = new Dictionary<string, List<string>>();

	public Task<bool> UserConnectd(string username, string connectionId)
	{
		bool isOnline = false;

		lock (OnLineUsers)
		{
			if (OnLineUsers.ContainsKey(username))
				OnLineUsers[username].Add(connectionId);
			else
			{
				OnLineUsers.Add(username, new List<string> { connectionId });
				isOnline = true;
			}
		}

		return Task.FromResult(isOnline);
	}

	public Task<bool> UserDisconnected(string username, string connectionId)
	{
		bool isOffline = false;
		lock (OnLineUsers)
		{
			if (!OnLineUsers.ContainsKey(username)) return Task.FromResult(false);

			OnLineUsers[username].Remove(connectionId);

			if (OnLineUsers[username].Count == 0)
			{
				OnLineUsers.Remove(username);
				isOffline = true;
			}
		}
		return Task.FromResult(isOffline);

	}

	public Task<string[]> GetOnlineUsers()
	{
		string[] onlineUsers;
		lock (OnLineUsers)
		{
			onlineUsers = OnLineUsers.OrderBy(c => c.Key).Select(c => c.Key).ToArray();
		}

		return Task.FromResult(onlineUsers);
	}

	public static Task<List<string>> GetConnectionsForUser(string username)
	{
		List<string> connectionIds;
		lock (OnLineUsers)
		{
			connectionIds = OnLineUsers.GetValueOrDefault(username);
		}

		return Task.FromResult(connectionIds);
	}


}

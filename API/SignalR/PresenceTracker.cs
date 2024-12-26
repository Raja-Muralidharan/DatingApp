using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlinceUsers = [];

        public Task<bool> UserConnected(string username, string connectedID)
        {
            var isOnline = false;

            lock (OnlinceUsers)
            {
                if(OnlinceUsers.ContainsKey(username))
                {
                    OnlinceUsers[username].Add(connectedID);
                }
                else
                {
                    OnlinceUsers.Add(username, [connectedID]);
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectedID)
        {

            var isOffline = false;
            lock(OnlinceUsers)
            {
                if(!OnlinceUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                OnlinceUsers[username].Remove(connectedID);

                if(OnlinceUsers[username].Count == 0)
                {
                    OnlinceUsers.Remove(username);
                    isOffline = true;
                }            
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlinceUsers)
            {
                onlineUsers = OnlinceUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionsIds;

            if(OnlinceUsers.TryGetValue(username,out var connections))
            {
                lock(connections)
                {
                    connectionsIds = connections.ToList();
                }
            }
            else
            {
                connectionsIds = [];
            }

            return Task.FromResult(connectionsIds);
        }

        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extenstions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub(PresenceTracker presenceTracker) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if(Context.User == null) throw new HubException("Cannot get current user claim");

            var isonline = await presenceTracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
            if(isonline) await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUserName());

            var currentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if(Context.User == null) throw new HubException("Cannot get current user claim");

            var isOffline = await presenceTracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);

            if(isOffline) await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUserName());
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
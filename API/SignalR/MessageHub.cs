using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extenstions;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub(IUnitofWork unitofWork, IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var otherUser = httpContext?.Request.Query["user"];

            if (Context.User == null || string.IsNullOrEmpty(otherUser))
            {
                throw new Exception("Cannot join Group");
            }

            var groupname = GetGroupName(Context.User.GetUserName(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupname);

            var group = await AddToGroup(groupname);

            await Clients.Group(groupname).SendAsync("UpdatedGroup", group);

            var messages = await unitofWork.messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser!);

            if (unitofWork.HasChanges()) await unitofWork.Complete();

            await Clients.Caller.SendAsync("ReciveMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User?.GetUserName() ?? throw new Exception("could not get user");

            if (username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You cannot message yourself");

            var sender = await unitofWork.userRepository.GetUserByUsernameAsync(username);
            var recipient = await unitofWork.userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null)
                throw new HubException("Cannot send message at this time");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await unitofWork.messageRepository.GetMessageGroup(groupName);

            if (group != null && group.Connections.Any(x => x.UserName == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null && connections?.Count != null)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }

            unitofWork.messageRepository.AddMessage(message);

            if (await unitofWork.Complete())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
            }
        }

        private async Task<Group> AddToGroup(string groupName)
        {

            var username = Context.User?.GetUserName() ?? throw new Exception("Cannot get user");

            var group = await unitofWork.messageRepository.GetMessageGroup(groupName);

            var connection = new Connections { ConnectionId = Context.ConnectionId, UserName = username };

            if (group == null)
            {
                group = new Group { Name = groupName };
                unitofWork.messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await unitofWork.Complete()) return group;

            throw new HubException("Failed to join group");

        }



        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await unitofWork.messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (connection != null && group != null)
            {
                unitofWork.messageRepository.RemoveConnection(connection);
                if (await unitofWork.Complete()) return group;
            }

            throw new HubException("Failed to remove from group");


        }

        private string GetGroupName(string caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
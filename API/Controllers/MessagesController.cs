using API.Controllers;
using API.DTOs;
using API.Extenstions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

[Authorize]
public class MessagesController(IMessageRepository messageRepository, 
             IUserRepository userRepository, IMapper mapper): BaseController
{
  
  [HttpPost]
  public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
  {
    var username = User.GetUserName();

    if(username == createMessageDto.RecipientUsername.ToLower())
      return BadRequest("You cannot message yourself");

    var sender = await userRepository.GetUserByUsernameAsync(username);
    var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

    if(recipient == null || sender == null || sender.UserName == null || recipient.UserName == null) 
          return BadRequest("Cannot send message at this time");

    var message = new Message
    {
        Sender = sender,
        Recipient = recipient,
        SenderUsername = sender.UserName,
        RecipientUsername = recipient.UserName,
        Content = createMessageDto.Content
    };

    messageRepository.AddMessage(message);

    if(await messageRepository.SaveAllAsyc()) return Ok(mapper.Map<MessageDto>(message));

    return BadRequest("Failed to save Messages");
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
  {
    messageParams.Username = User.GetUserName();

    var messages = await messageRepository.GetMessagesForUser(messageParams);

    Response.AddPaginationHeader(messages);

    return messages;
  }

  [HttpGet("thread/{username}")]
  public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
  {
    var currentUsername = User.GetUserName();

    return Ok(await messageRepository.GetMessageThread(currentUsername, username));
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteMessage(int id){

    var username = User.GetUserName();

    var message = await messageRepository.GetMessage(id);

    if(message == null) return BadRequest("Cannot delete this message");

    if(message.SenderUsername != username && message.RecipientUsername != username) return Forbid();

    if(message.SenderUsername == username) message.SenderDeleted = true;

    if(message.RecipientUsername == username) message.RecipientDeleted = true;

    if(message is {SenderDeleted: true, RecipientDeleted: true}){
      messageRepository.DeleteMessage(message);
    }

    if(await messageRepository.SaveAllAsyc()) return Ok();

    return BadRequest("Problem deleting the message");
  }
}
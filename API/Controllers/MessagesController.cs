using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, 
            IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

       [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername(); // lay username tu token

            // kiem tra nguoi gui va nguoi nhan co giong nhau hay khong  neu giong nhau thi tra ve loi
            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself"); 

            var sender = await _userRepository.GetUserByUsernameAsync(username); // get the sender 

            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername); // get the recipient

            if (recipient == null) return NotFound(); // if the recipient is not found 

            // create a new message 
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser ([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername(); // lay username tu token

            var messages = await _messageRepository.GetMessagesForUser(messageParams); // lay danh sach tin nhan cua nguoi dung

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, 
                messages.TotalCount, messages.TotalPages); // them header phan trang vao response

            return messages;
        }

       [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

         [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername(); // lay username tu token

            var message = await _messageRepository.GetMessage(id); // lay tin nhan theo id

             // kiem tra nguoi gui va nguoi nhan co giong nhau hay khong  neu giong nhau thi tra ve loi 401
            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();

            // neu nguoi gui xoa tin nhan thi set SenderDeleted = true
            if (message.Sender.UserName == username) message.SenderDeleted = true; 

            // neu nguoi nhan xoa tin nhan thi set RecipientDeleted = true
            if (message.Recipient.UserName == username) message.RecipientDeleted = true; 

             // neu ca nguoi gui va nguoi nhan deu xoa tin nhan thi xoa tin nhan khoi database
            if (message.SenderDeleted && message.RecipientDeleted) 
                _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}
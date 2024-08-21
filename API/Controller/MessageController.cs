using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller
{
    public class MessageController : BaseApiController
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageController(IAppUserRepository appUserRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _appUserRepository = appUserRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {

            var username = User.GetUserName();
            if (username.ToLower() == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("Não pode mandar pra vc");

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

            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Erro ao enviar a mensagem");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            var messages = await _messageRepository.GetMessagessForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUser = User.GetUserName();
            return Ok(await _messageRepository.GetMessageThread(currentUser, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {

            var username = User.GetUserName();
            var message = await _messageRepository.GetMessage(id);
            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.SenderUsername == username) message.SenderDeletete = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeletete && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest();


        }
    }
}

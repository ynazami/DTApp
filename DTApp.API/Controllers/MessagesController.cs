using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DTApp.API.Data;
using DTApp.API.DTO;
using DTApp.API.Helper;
using DTApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTApp.API.Controllers
{

    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly IDatingRepository _repository;
        public MessagesController(IDatingRepository repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

        [HttpGet("{id}", Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repository.GetMessage(id);

            if(messageFromRepo == null)
            {
                return NotFound();
            }

            if(messageFromRepo.RecipientId == userId || messageFromRepo.SenderId == userId)
            {
                return Ok(messageFromRepo);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repository.GetMessage(id);

            if(messageFromRepo == null)
            {
                return NotFound();
            }

            if(messageFromRepo.RecipientId != userId) 
            {
                return Unauthorized();
            }

            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.Now;

            await _repository.SaveAll();
            return NoContent();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {
            var sender = await _repository.GetUser(userId);
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repository.GetMessage(id);

            if(messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }

            if(messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            if(messageFromRepo.RecipientDeleted && messageFromRepo.SenderDeleted)
            {
                _repository.Delete(messageFromRepo);
            }

            if(await _repository.SaveAll())
            {
                return NoContent();
            }
            throw new Exception("Message deletion failed.");
        }
         
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageDto)
        {
            var sender = await _repository.GetUser(userId);
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageDto.SenderId = userId;

            var recipient = await _repository.GetUser(messageDto.RecipientId);

            if(recipient == null)
            {
                return BadRequest("Recipient not found");
            }

            var message = _mapper.Map<Message>(messageDto);

            _repository.Add(message);

            if(await _repository.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {Id = message.Id}, messageToReturn);
            }
            throw new Exception("Message save failed"); 
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessagesConversation(int userId, int recipientId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var recipient = await _repository.GetUser(recipientId);

            if(recipient == null)
            {
                return BadRequest("Recipient not found");
            }

            var messagesFromRepo = await _repository.GetConversation(userId,recipientId);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            return Ok(messages);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;
            var messagesFromRepo = await _repository.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            Response.AddPaginationHeader(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotlaPages);
            return Ok(messages);
        }
    }
}
using ChatApi.BLL.Basic;
using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Chats.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Chats.Concrete
{
    public class ChatService : BaseValidation, IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public CreateChatResponseDto? Create(CreateChatRequestDto createRequestDto)
        {
            if (!ValidateObject(createRequestDto))
            {
                return null;
            }
            var existChat = _chatRepository.FindByName(createRequestDto.Name);
            if (existChat != null)
            {
                AddValidationError(
                    errorMessage: "chat with the same name already exists",
                    memberName: nameof(createRequestDto.Name));
                return null;
            }
            Entities.Chat chat = new Entities.Chat
            {
                ChatId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Name = createRequestDto.Name
            };
            _chatRepository.Add(chat);
            return new CreateChatResponseDto
            {
                ChatId = chat.ChatId
            };
        }
    }
}

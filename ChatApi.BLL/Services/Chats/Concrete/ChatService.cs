using ChatApi.BLL.Basic;
using ChatApi.BLL.Entities;
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
        private readonly IUserRepository _userRepository;

        public ChatService(
            IChatRepository chatRepository,
            IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
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
            Chat chat = new Chat
            {
                ChatId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Name = createRequestDto.Name
            };
            var userIds = createRequestDto.Users;
            if (userIds.Distinct().Count() < userIds.Count())
            {
                AddValidationError(
                    errorMessage: "users contain duplicates",
                    memberName: nameof(createRequestDto.Users));
                return null;
            }
            if (userIds.Distinct().Count() != userIds.Count())
            {
                AddValidationError(
                    errorMessage: "users contain duplicates",
                    memberName: nameof(createRequestDto.Users));
                return null;
            }
            var users = _userRepository.GetByIds(userIds)
                .ToDictionary(keySelector: u => u.UserId);
            foreach (var userId in userIds)
            {
                if (!users.ContainsKey(userId))
                {
                    AddValidationError(
                        errorMessage: $"user not found: id='{userId}'",
                        memberName: nameof(createRequestDto.Users));
                    return null;
                }
                User user = users[userId];
                chat.Users.Add(user);
            }
            _chatRepository.Add(chat);
            return new CreateChatResponseDto
            {
                ChatId = chat.ChatId
            };
        }

        public IEnumerable<ChatMessageItemDto>? GetChatMessages(ChatMessagesRequestDto chatMessagesRequestDto)
        {
            if (!ValidateObject(chatMessagesRequestDto))
            {
                return null;
            }
            Chat? chatWithMessages = _chatRepository.GetChatWithMessagesOrderedByCreatedAt(chatId: chatMessagesRequestDto.Chat);
            if (chatWithMessages == null)
            {
                AddValidationError(
                    errorMessage: "chat not found", 
                    memberName: $"chatId='{chatMessagesRequestDto.Chat}'");
                return null;
            }
            return chatWithMessages.Messages.Select(m => new ChatMessageItemDto
            {
                Id = m.MessageId,
                Text = m.Text,
                AuthorId = m.User.UserId,
                AuthorName = m.User.UserName,
                CreatedAt = m.CreatedAt.ToString(format: "yyyy-MM-dd HH:mm:ss")
            });
        }

        public ChatsOfUserResponseDto? GetUserChats(ChatsOfUserRequestDto chatsRequestDto)
        {
            var chats = _chatRepository.FindByUser(chatsRequestDto.User);
            if (chats == null || !chats.Any())
            {
                return null;
            }
            return new ChatsOfUserResponseDto
            {
                Chats = chats.Select(c => new ChatItemDto
                {
                    Id = c.ChatId,
                    Name = c.Name,
                    CreatedAt = c.CreatedAt.ToString(format: "yyyy-MM-dd HH:mm:ss")
                })
            };
        }
    }
}

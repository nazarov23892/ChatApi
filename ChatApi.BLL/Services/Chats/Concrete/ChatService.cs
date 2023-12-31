﻿using ChatApi.BLL.Basic;
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
                    errorMessage: $"chat not found: chatId='{chatMessagesRequestDto.Chat}'",
                    memberName: "-");
                return null;
            }
            return chatWithMessages.Messages.Select(m => new ChatMessageItemDto
            {
                Id = m.MessageId,
                Text = m.Text,
                AuthorId = m.Author?.UserId ?? string.Empty,
                AuthorName = m.Author?.UserName ?? string.Empty,
                CreatedAt = m.CreatedAt.ToString(format: "yyyy-MM-dd HH:mm:ss")
            });
        }

        public ChatsOfUserResponseDto? GetUserChats(ChatsOfUserRequestDto chatsRequestDto)
        {
            if (!ValidateObject(chatsRequestDto))
            {
                return null;
            }
            var user = _chatRepository.GetUserWithChats(chatsRequestDto.User);
            if (user == null)
            {
                AddValidationError(
                    errorMessage: $"user not found: Id='{chatsRequestDto.User}'",
                    memberName: "-");
                return null;
            }
            return new ChatsOfUserResponseDto
            {
                Chats = user.Chats.Select(c => new ChatItemDto
                {
                    Id = c.ChatId,
                    Name = c.Name,
                    CreatedAt = c.CreatedAt.ToString(format: "yyyy-MM-dd HH:mm:ss")
                })
            };
        }

        public PostMessageResponse? PostMessage(PostMessageRequestDto postMessageRequest)
        {
            if (!ValidateObject(postMessageRequest))
            {
                return null;
            }
            User? user = _userRepository.GetUser(userId: postMessageRequest.Author);
            if (user == null)
            {
                AddValidationError(
                    errorMessage: $"user not found: Id='{postMessageRequest.Author}'",
                    memberName: "-");
                return null;
            }
            Chat? chat = _chatRepository.GetChatWithUsers(chatId: postMessageRequest.Chat);
            if (chat == null)
            {
                AddValidationError(
                    errorMessage: $"chat not found: Id='{postMessageRequest.Chat}'",
                    memberName: "-");
                return null;
            }
            if (!chat.Users
                .Any(u => postMessageRequest.Author.Equals(u.UserId, StringComparison.OrdinalIgnoreCase)))
            {
                AddValidationError(
                    errorMessage: $"chat does not have this user: Id='{postMessageRequest.Author}'",
                    memberName: "-");
                return null;
            }
            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Text = postMessageRequest.Text,
                ChatId = chat.ChatId,
                AuthorId = postMessageRequest.Author,
                CreatedAt = DateTime.Now
            };
            _chatRepository.AddMessage(message);
            return new PostMessageResponse
            {
                Id = message.MessageId
            };
        }
    }
}

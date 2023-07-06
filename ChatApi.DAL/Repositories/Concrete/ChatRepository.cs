using ChatApi.BLL.Entities;
using ChatApi.BLL.Repositories;
using ChatApi.DAL.DataContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.DAL.Repositories.Concrete
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDataContext _efDbContext;

        public ChatRepository(AppDataContext efDbContext)
        {
            _efDbContext = efDbContext;
        }

        public void Add(Chat chat)
        {
            if (string.IsNullOrEmpty(chat.ChatId))
            {
                throw new ArgumentException(message: $"chat.{nameof(chat.ChatId)} cannot be null or empty");
            }
            _efDbContext.Chats.Add(chat);
            _efDbContext.SaveChanges();
        }

        public Chat? FindByName(string name)
        {
            return _efDbContext.Chats
                .SingleOrDefault(c => name.Equals(c.Name, StringComparison.OrdinalIgnoreCase));
        }

        public User? GetUserWithChats(string userId)
        {
            return _efDbContext.Users
                .Include(u => u.Chats
                    .OrderByDescending(c => c.Messages.Max(m => (DateTime?)m.CreatedAt)))
                .SingleOrDefault(u => userId.Equals(u.UserId, StringComparison.OrdinalIgnoreCase));
        }

        public Chat? GetChatWithMessagesOrderedByCreatedAt(string chatId)
        {
            return _efDbContext.Chats
                .Select(c => new Chat
                {
                    ChatId = c.ChatId,
                    Messages = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => new Message
                        {
                            MessageId = m.MessageId,
                            Text = m.Text,
                            CreatedAt = m.CreatedAt,
                            Author = m.Author
                        })
                        .ToArray()
                })
                .SingleOrDefault(c => c.ChatId == chatId);
        }
    }
}

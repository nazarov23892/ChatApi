﻿using ChatApi.BLL.Entities;
using ChatApi.BLL.Repositories;
using ChatApi.DAL.DataContexts;
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

        public IEnumerable<Chat> FindByUser(string userId)
        {
            return _efDbContext.Chats
                .Where(c => c.Users
                        .Where(u => u.UserId.Equals(userId))
                        .Any())
                .ToArray();
        }
    }
}

using ChatApi.BLL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Repositories
{
    public interface IChatRepository
    {
        void Add(Chat chat);
        Chat? FindByName(string name);
        User? GetUserWithChats(string userId);
        Chat? GetChatWithMessagesOrderedByCreatedAt(string chatId);
    }
}

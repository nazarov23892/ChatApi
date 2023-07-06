using ChatApi.BLL.Entities;
using ChatApi.DAL.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.DAL.SeedData
{
    public static class SeedDataTool
    {
        public static void SeedData(AppDataContext appDataContext)
        {
            if (appDataContext.Users.Any() || appDataContext.Chats.Any())
            {
                return;
            }

            User user1 = new User
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000001").ToString(),
                UserName = "user_1",
                CreatedAt = DateTime.Parse("2023-03-16 12:01:41")
            };
            User user2 = new User
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000002").ToString(),
                UserName = "user_2",
                CreatedAt = DateTime.Parse("2023-01-12 15:27:16")
            };
            User user3 = new User
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000003").ToString(),
                UserName = "user_3",
                CreatedAt = DateTime.Parse("2022-02-24 04:01:15")
            };

            Chat chat1 = new Chat
            {
                ChatId = new Guid("00000000-0000-0000-0000-000000000001").ToString(),
                Name = "chat_1",
                CreatedAt = DateTime.Parse("2023-06-20 11:16:22"),
                Users = new List<User> { user1, user2 },
                Messages = new List<Message>
                {
                    new Message { MessageId = "01", Author = user1, Text = "user1-message1", CreatedAt = DateTime.Parse("2023-06-21 13:00:00") },
                    new Message { MessageId = "02", Author = user1, Text = "user1-message2", CreatedAt = DateTime.Parse("2023-06-22 13:06:11") },
                }
            };

            Chat chat2 = new Chat
            {
                ChatId = new Guid("00000000-0000-0000-0000-000000000002").ToString(),
                Name = "chat_2",
                CreatedAt = DateTime.Parse("2023-04-10 13:17:09"),
                Users = new List<User> { user1, user3 },
                Messages = new List<Message>
                {
                    new Message { MessageId = "03", Author = user3, Text = "user3-message1", CreatedAt = DateTime.Parse("2023-06-21 13:00:00") },
                    new Message { MessageId = "04", Author = user3, Text = "user3-message2", CreatedAt = DateTime.Parse("2023-06-23 13:06:11") },
                }
            };

            Chat chat3 = new Chat
            {
                ChatId = new Guid("00000000-0000-0000-0000-000000000003").ToString(),
                Name = "chat_3",
                CreatedAt = DateTime.Parse("2023-01-11 17:16:11"),
                Users = new List<User> { user1, user3 }
            };

            appDataContext.Chats.Add(chat1);
            appDataContext.Chats.Add(chat2);
            appDataContext.Chats.Add(chat3);
            appDataContext.SaveChanges();
        }
    }
}

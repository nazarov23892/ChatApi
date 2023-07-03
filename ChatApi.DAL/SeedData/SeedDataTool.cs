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
                CreatedAt = new DateTime(
                    year: 2023,
                    month: 3,
                    day: 16,
                    hour: 12,
                    minute: 1,
                    second: 41)
            };
            User user2 = new User
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000002").ToString(),
                UserName = "user_2",

                CreatedAt = new DateTime(
                    year: 2023,
                    month: 1,
                    day: 12,
                    hour: 15,
                    minute: 27,
                    second: 16)
            };
            User user3 = new User
            {
                UserId = new Guid("00000000-0000-0000-0000-000000000003").ToString(),
                UserName = "user_3",
                CreatedAt = new DateTime(
                    year: 2022,
                    month: 02,
                    day: 24,
                    hour: 4,
                    minute: 1,
                    second: 15)
            };

            Chat chat1 = new Chat
            {
                ChatId = new Guid("00000000-0000-0000-0000-000000000001").ToString(),
                Name = "chat_1",
                CreatedAt = new DateTime(
                    year: 2023,
                    month: 6,
                    day: 20,
                    hour: 11,
                    minute: 16,
                    second: 22),
                Users = new List<User> { user1, user2 }
            };

            Chat chat2 = new Chat
            {
                ChatId = new Guid("00000000-0000-0000-0000-000000000002").ToString(),
                Name = "chat_2",
                CreatedAt = new DateTime(
                    year: 2023,
                    month: 4,
                    day: 10,
                    hour: 13,
                    minute: 17,
                    second: 9),
                Users = new List<User> { user1, user3 }
            };

            appDataContext.Chats.Add(chat1);
            appDataContext.Chats.Add(chat2);
            appDataContext.SaveChanges();
        }
    }
}

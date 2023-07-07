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
    public class UserRepository : IUserRepository
    {
        private readonly AppDataContext _efDbContext;

        public UserRepository(AppDataContext efDbContext)
        {
            _efDbContext = efDbContext;
        }

        public void Add(User user)
        {
            if (string.IsNullOrEmpty(user.UserId))
            {
                throw new ArgumentException(message: $"user.{nameof(user.UserId)} cannot be null or empty");
            }
            _efDbContext.Users.Add(user);
            _efDbContext.SaveChanges();
        }

        public User? FindByName(string username)
        {
            return _efDbContext.Users
                .SingleOrDefault(u => EF.Functions.Like(u.UserName, $"%{username}%"));
        }

        public IEnumerable<User> GetByIds(IEnumerable<string> userIds)
        {
            return _efDbContext.Users
                .Where(u => userIds.Contains(u.UserId))
                ?.ToArray()
                ?? Enumerable.Empty<User>();
        }

        public User? GetUser(string userId)
        {
            return _efDbContext.Users
                .SingleOrDefault(u => EF.Functions.Like(u.UserId, $"%{userId}%"));
        }
    }
}

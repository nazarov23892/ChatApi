using ChatApi.BLL.Entities;
using ChatApi.BLL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.DAL.Repositories.Concrete
{
    public class UserRepository : IUserRepository
    {
        private readonly Dictionary<string, User> _users = new Dictionary<string, User>();

        public void Add(User user)
        {
            if (user.UserId == null)
            {
                throw new ArgumentException(message: $"user.{nameof(user.UserId)} cannot be null");
            }
            if (_users.ContainsKey(user.UserId))
            {
                throw new ArgumentException(message: "user already exist");
            }
            _users[user.UserId] = user;
        }
    }
}

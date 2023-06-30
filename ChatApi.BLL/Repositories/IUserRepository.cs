using ChatApi.BLL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Repositories
{
    public interface IUserRepository
    {
        void Add(User user);
        User? FindByName(string username);
    }
}

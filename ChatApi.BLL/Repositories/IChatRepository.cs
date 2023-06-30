using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Repositories
{
    public interface IChatRepository
    {
        void Add(Entities.Chat chat);
    }
}

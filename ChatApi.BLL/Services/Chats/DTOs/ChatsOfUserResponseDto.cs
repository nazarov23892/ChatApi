using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Chats.DTOs
{
    public class ChatsOfUserResponseDto
    {
        public IEnumerable<ChatItemDto> Chats { get; set; } = Array.Empty<ChatItemDto>();
    }
}

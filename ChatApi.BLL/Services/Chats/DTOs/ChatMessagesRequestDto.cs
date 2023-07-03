using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Chats.DTOs
{
    public class ChatMessagesRequestDto
    {
        [Required]
        public string Chat { get; set; } = string.Empty;
    }
}

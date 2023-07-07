using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Chats.DTOs
{
    public class ChatsOfUserRequestDto
    {
        [Required]
        public string User { get; set; } = string.Empty;
    }
}

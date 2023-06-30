using ChatApi.BLL.Services.Chats.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Chats
{
    public interface IChatService
    {
        CreateChatResponseDto? Create(CreateChatRequestDto createRequestDto);
        bool HasValidationProblems { get; }
        Dictionary<string, string[]> ValidationProblems { get; }
    }
}

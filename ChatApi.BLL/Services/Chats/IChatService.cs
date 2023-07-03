using ChatApi.BLL.Basic;
using ChatApi.BLL.Services.Chats.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Chats
{
    public interface IChatService: IValidatableService
    {
        CreateChatResponseDto? Create(CreateChatRequestDto createRequestDto);
        ChatsOfUserResponseDto? GetUserChats(ChatsOfUserRequestDto chatsRequestDto);
        IEnumerable<ChatMessageItemDto>? GetChatMessages(ChatMessagesRequestDto chatMessagesRequestDto);
    }
}

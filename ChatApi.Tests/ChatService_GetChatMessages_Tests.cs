using ChatApi.BLL.Entities;
using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Chats.Concrete;
using ChatApi.BLL.Services.Chats.DTOs;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Tests
{
    public class ChatService_GetChatMessages_Tests
    {
        [Fact]
        public void Cannot_Get_When_ChatId_Not_Found()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            chatRepositoryMock
                .Setup(m => m.GetChatWithMessagesOrderedByCreatedAt(It.IsAny<string>()))
                .Returns<string>(chatId => chatId.Equals("001") ? new Chat { ChatId = "001" } : null);

            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);
            ChatMessagesRequestDto chatMessagesRequestDto = new ChatMessagesRequestDto { Chat = "002" };

            // act

            var response = target.GetChatMessages(chatMessagesRequestDto);

            // assert

            Assert.Null(response);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "chat not found",
                actualString: target.ValidationProblems.Single());
        }

        [Fact]
        public void Can_Get()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            chatRepositoryMock
                .Setup(m => m.GetChatWithMessagesOrderedByCreatedAt(It.IsAny<string>()))
                .Returns<string>(chatId => chatId.Equals("001")
                    ? new Chat
                    {
                        ChatId = "001",
                        Messages = new List<BLL.Entities.Message>
                        {
                            new BLL.Entities.Message { MessageId = "0001" },
                            new BLL.Entities.Message { MessageId = "0002" },
                            new BLL.Entities.Message { MessageId = "0003" }
                        }
                    }
                    : null);


            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);
            ChatMessagesRequestDto chatMessagesRequestDto = new ChatMessagesRequestDto { Chat = "001" };

            // act

            var response = target.GetChatMessages(chatMessagesRequestDto);

            // assert

            Assert.NotNull(response);
            Assert.False(target.HasValidationProblems);
            Assert.Empty(target.ValidationProblems);

            var chats = response.ToArray();

            Assert.Equal(
                expected: 3,
                actual: chats.Length);

            Assert.Equal(expected: "0001", actual: chats[0].Id);
            Assert.Equal(expected: "0002", actual: chats[1].Id);
            Assert.Equal(expected: "0003", actual: chats[2].Id);
        }
    }
}

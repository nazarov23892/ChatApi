using ChatApi.BLL.Entities;
using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Chats.Concrete;
using ChatApi.BLL.Services.Chats.DTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ChatApi.Tests
{
    public class ChatService_GetUserChats_Tests
    {
        [Fact]
        public void Cannot_Get_When_UserId_Not_Found()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            chatRepositoryMock
                .Setup(m => m.GetUserWithChats(It.IsAny<string>()))
                .Returns<string>(userId => userId.Equals("001") ? new User { UserId = "001" } : null);

            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);
            ChatsOfUserRequestDto chatsOfUserRequestDto = new ChatsOfUserRequestDto { User = "002" };

            // act

            ChatsOfUserResponseDto? response = target.GetUserChats(chatsOfUserRequestDto);

            // arrange

            Assert.Null(response);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "user not found",
                actualString: target.ValidationProblems.Single());
        }

        [Fact]
        public void Can_Get()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            chatRepositoryMock
                .Setup(m => m.GetUserWithChats(It.IsAny<string>()))
                .Returns<string>(userId => userId.Equals("001")
                    ? new User
                    {
                        UserId = "001",
                        Chats = new List<Chat>
                        {
                            new Chat { ChatId = "0001" },
                            new Chat { ChatId = "0002" },
                            new Chat { ChatId = "0003" }
                        }
                    }
                    : null);

            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);
            ChatsOfUserRequestDto chatsOfUserRequestDto = new ChatsOfUserRequestDto { User = "001" };

            // act

            ChatsOfUserResponseDto? response = target.GetUserChats(chatsOfUserRequestDto);

            // arrange

            Assert.NotNull(response);
            Assert.False(target.HasValidationProblems);
            Assert.Empty(target.ValidationProblems);

            var chats = response.Chats.ToArray();

            Assert.Equal(
                expected: 3,
                actual: chats.Length);

            Assert.Equal(expected: "0001", actual: chats[0].Id);
            Assert.Equal(expected: "0002", actual: chats[1].Id);
            Assert.Equal(expected: "0003", actual: chats[2].Id);
        }
    }
}

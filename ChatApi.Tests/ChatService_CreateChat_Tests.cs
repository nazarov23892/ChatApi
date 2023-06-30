using ChatApi.BLL.Entities;
using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Chats.Concrete;
using ChatApi.BLL.Services.Chats.DTOs;
using ChatApi.BLL.Services.Users.Concrete;
using ChatApi.BLL.Services.Users.DTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Tests
{
    public class ChatService_CreateChat_Tests
    {
        [Fact]
        public void Cannot_Create_Chat_When_Chatrname_Is_Empty()
        {
            // arrange

            Mock<IChatRepository> mock = new Mock<IChatRepository>();

            ChatService target = new ChatService(mock.Object);

            // act

            CreateChatResponseDto? result = target.Create(new CreateChatRequestDto
            {
                Name = string.Empty,
                Users = new[] { "aaa", "bbb" }
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "Name field is required",
                actualString: target.ValidationProblems.Keys.Single());
            mock.Verify(
                expression: m => m.Add(It.IsAny<Chat>()),
                times: Times.Never);
        }
    }
}

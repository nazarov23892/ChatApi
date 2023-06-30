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

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);

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
            chatRepositoryMock.Verify(
                expression: m => m.Add(It.IsAny<Chat>()),
                times: Times.Never);
        }

        [Fact]
        public void Cannot_Create_Chat_When_Chatname_Is_Less_Than_3()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);

            // act

            CreateChatResponseDto? result = target.Create(new CreateChatRequestDto
            {
                Name = "ab",
                Users = new[] { "aaa", "bbb" }
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "Name must be",
                actualString: target.ValidationProblems.Keys.Single());
            Assert.Contains(
                expectedSubstring: "minimum length of 3",
                actualString: target.ValidationProblems.Keys.Single());
            chatRepositoryMock.Verify(
                expression: m => m.Add(It.IsAny<Chat>()),
                times: Times.Never);
        }

        [Fact]
        public void Cannot_Create_Chat_When_Chatname_Is_Longer_Than_20()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);

            // act

            CreateChatResponseDto? result = target.Create(new CreateChatRequestDto
            {
                Name = "aaaaaaaaaaaaaaaaaaaaa",
                Users = new[] { "aaa", "bbb" }
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "Name must be",
                actualString: target.ValidationProblems.Keys.Single());
            Assert.Contains(
                expectedSubstring: "minimum length of 3",
                actualString: target.ValidationProblems.Keys.Single());
            chatRepositoryMock.Verify(
                expression: m => m.Add(It.IsAny<Chat>()),
                times: Times.Never);
        }

        [Fact]
        public void Cannot_Create_Chat_When_Chatname_Contains_Symbol()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            CreateChatRequestDto[] requests = new[]
            {
                new CreateChatRequestDto{Name = " ab", Users = new[] { "aaa", "bbb" }}, // starts with symbol
                new CreateChatRequestDto{Name = "a-b", Users = new[] { "aaa", "bbb" }}, // contains symbol
                new CreateChatRequestDto{Name = "ab-", Users = new[] { "aaa", "bbb" }}, // ends with symbol
                new CreateChatRequestDto{Name = "***", Users = new[] { "aaa", "bbb" }},
            };

            foreach (var request in requests)
            {
                ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);

                // act

                CreateChatResponseDto? result = target.Create(request);

                // assert

                Assert.Null(result);
                Assert.True(target.HasValidationProblems);
                Assert.Single(target.ValidationProblems);
                Assert.Contains(
                    expectedSubstring: "Name must start with a letter and can only contain letters and numbers",
                    actualString: target.ValidationProblems.Keys.Single());
                chatRepositoryMock.Verify(
                    expression: m => m.Add(It.IsAny<Chat>()),
                    times: Times.Never);
            }
        }

        [Fact]
        public void Cannot_Create_Chat_When_Chatname_Is_Exist()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            chatRepositoryMock.Setup(expression: m => m.FindByName(It.IsAny<string>()))
                .Returns<string>(
                    valueFunction: s =>
                        s.Equals("chat1", comparisonType: StringComparison.OrdinalIgnoreCase)
                        ? new Chat
                        {
                            ChatId = "001",
                            Name = "chat1"
                        }
                        : null);

            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);

            // act

            CreateChatResponseDto? result = target.Create(new CreateChatRequestDto
            {
                Name = "chat1",
                Users = new[] { "aaa", "bbb" }
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "the same name already exists",
                actualString: target.ValidationProblems.Keys.Single());
            chatRepositoryMock.Verify(
                expression: m => m.Add(It.IsAny<Chat>()),
                times: Times.Never);
        }
    }
}

﻿using ChatApi.BLL.Entities;
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
                actualString: target.ValidationProblems.Single());
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
                actualString: target.ValidationProblems.Single());
            Assert.Contains(
                expectedSubstring: "minimum length of 3",
                actualString: target.ValidationProblems.Single());
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
                actualString: target.ValidationProblems.Single());
            Assert.Contains(
                expectedSubstring: "minimum length of 3",
                actualString: target.ValidationProblems.Single());
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
                    actualString: target.ValidationProblems.Single());
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
                actualString: target.ValidationProblems.Single());
            chatRepositoryMock.Verify(
                expression: m => m.Add(It.IsAny<Chat>()),
                times: Times.Never);
        }

        [Fact]
        public void Cannot_Create_Chat_When_Users_Number_Less_2()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            chatRepositoryMock
                .Setup(expression: m => m.FindByName(It.IsAny<string>()))
                .Returns<string>(valueFunction: s => null);

            CreateChatRequestDto[] requests = new[]
            {
                new CreateChatRequestDto{Name = "abc1", Users = Array.Empty<string>()},
                new CreateChatRequestDto{Name = "abc2", Users = new[] { "aaa" }}
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
                    expectedSubstring: "Users must be",
                    actualString: target.ValidationProblems.Single());
                Assert.Contains(
                    expectedSubstring: "with a minimum length",
                    actualString: target.ValidationProblems.Single());
                chatRepositoryMock.Verify(
                    expression: m => m.Add(It.IsAny<Chat>()),
                    times: Times.Never);
            }
        }

        [Fact]
        public void Cannot_Create_Chat_When_Users_Has_Nonexist()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            chatRepositoryMock
                .Setup(expression: m => m.FindByName(It.IsAny<string>()))
                .Returns<string>(valueFunction: s => null);

            userRepositoryMock
                .Setup(expression: m => m.GetByIds(It.IsAny<IEnumerable<string>>()))
                .Returns<IEnumerable<string>>(valueFunction: s => new[]
                {
                    new User { UserId = "001", UserName = "user1" },
                    new User { UserId = "002", UserName = "user2" },
                });

            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);

            // act

            CreateChatResponseDto? result = target.Create(new CreateChatRequestDto
            {
                Name = "chat1",
                Users = new[] { "001", "002", "003" }
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "user not found",
                actualString: target.ValidationProblems.Single());
            chatRepositoryMock.Verify(
                expression: m => m.Add(It.IsAny<Chat>()),
                times: Times.Never);
        }

        [Fact]
        public void Cannot_Create_Chat_When_Users_Have_Duplicate()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            chatRepositoryMock
                .Setup(expression: m => m.FindByName(It.IsAny<string>()))
                .Returns<string>(valueFunction: s => null);

            ChatService target = new ChatService(chatRepositoryMock.Object, userRepositoryMock.Object);

            // act

            CreateChatResponseDto? result = target.Create(new CreateChatRequestDto
            {
                Name = "chat1",
                Users = new[] { "aaa", "bbb", "aaa" }
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "users contain duplicates",
                actualString: target.ValidationProblems.Single());
            chatRepositoryMock.Verify(
                expression: m => m.Add(It.IsAny<Chat>()),
                times: Times.Never);
        }
    }
}

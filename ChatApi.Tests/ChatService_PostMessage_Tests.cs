﻿using ChatApi.BLL.Services.Chats.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Chats.DTOs;
using ChatApi.BLL.Entities;

namespace ChatApi.Tests
{
    public class ChatService_PostMessage_Tests
    {
        [Fact]
        public void Cannot_PostMessage_When_Author_Is_Empty()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            ChatService target = new ChatService(
                chatRepositoryMock.Object,
                userRepositoryMock.Object);

            PostMessageRequestDto postMessageRequest = new PostMessageRequestDto
            {
                Author = string.Empty,
                Chat = "01",
                Text = "text"
            };

            // act

            PostMessageResponse? response = target.PostMessage(postMessageRequest);

            // assert

            Assert.Null(response);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "author field is required",
                actualString: target.ValidationProblems.Single(),
                comparisonType: StringComparison.OrdinalIgnoreCase
                );

            chatRepositoryMock.Verify(
                expression: m => m.AddMessage(It.IsAny<Message>()),
                times: Times.Never
                );
        }

        [Fact]
        public void Cannot_PostMessage_When_Chat_Is_Empty()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            ChatService target = new ChatService(
                chatRepositoryMock.Object,
                userRepositoryMock.Object);

            PostMessageRequestDto postMessageRequest = new PostMessageRequestDto
            {
                Author = "01",
                Chat = string.Empty,
                Text = "text"
            };

            // act

            PostMessageResponse? response = target.PostMessage(postMessageRequest);

            // assert

            Assert.Null(response);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "chat field is required",
                actualString: target.ValidationProblems.Single(),
                comparisonType: StringComparison.OrdinalIgnoreCase
                );

            chatRepositoryMock.Verify(
                expression: m => m.AddMessage(It.IsAny<Message>()),
                times: Times.Never
                );
        }

        [Fact]
        public void Cannot_PostMessage_When_Text_Is_Empty()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            ChatService target = new ChatService(
                chatRepositoryMock.Object,
                userRepositoryMock.Object);

            PostMessageRequestDto postMessageRequest = new PostMessageRequestDto
            {
                Author = "01",
                Chat = "01",
                Text = string.Empty
            };

            // act

            PostMessageResponse? response = target.PostMessage(postMessageRequest);

            // assert

            Assert.Null(response);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "text field is required",
                actualString: target.ValidationProblems.Single(),
                comparisonType: StringComparison.OrdinalIgnoreCase
                );

            chatRepositoryMock.Verify(
                expression: m => m.AddMessage(It.IsAny<Message>()),
                times: Times.Never
                );
        }

        [Fact]
        public void Cannot_PostMessage_When_User_Not_Exist()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(m => m.GetUser(It.IsAny<string>()))
                .Returns<string>(valueFunction: userId => userId.Equals("01")
                    ? new User
                    {
                        UserId = "01",
                    }
                    : null);

            ChatService target = new ChatService(
                chatRepositoryMock.Object,
                userRepositoryMock.Object);

            PostMessageRequestDto postMessageRequest = new PostMessageRequestDto
            {
                Author = "02",
                Chat = "01",
                Text = "text"
            };

            // act

            PostMessageResponse? response = target.PostMessage(postMessageRequest);

            // assert

            Assert.Null(response);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "user not found",
                actualString: target.ValidationProblems.Single(),
                comparisonType: StringComparison.OrdinalIgnoreCase
                );

            chatRepositoryMock.Verify(
                expression: m => m.AddMessage(It.IsAny<Message>()),
                times: Times.Never
                );
        }

        [Fact]
        public void Cannot_PostMessage_When_Chat_Not_Exist()
        {
            // arrange

            Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(m => m.GetUser(It.IsAny<string>()))
                .Returns<string>(valueFunction: userId => userId.Equals("01")
                    ? new User
                    {
                        UserId = "01",
                    }
                    : null);

            chatRepositoryMock
                .Setup(m => m.GetChatWithUsers(It.IsAny<string>()))
                .Returns<string>(valueFunction: chatId => chatId.Equals("01")
                    ? new Chat
                    {
                        ChatId = "01"
                    }
                    : null);

            ChatService target = new ChatService(
                chatRepositoryMock.Object,
                userRepositoryMock.Object);

            PostMessageRequestDto postMessageRequest = new PostMessageRequestDto
            {
                Author = "01",
                Chat = "02",
                Text = "text"
            };

            // act

            PostMessageResponse? response = target.PostMessage(postMessageRequest);

            // assert

            Assert.Null(response);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "chat not found",
                actualString: target.ValidationProblems.Single(),
                comparisonType: StringComparison.OrdinalIgnoreCase
                );

            chatRepositoryMock.Verify(
                expression: m => m.AddMessage(It.IsAny<Message>()),
                times: Times.Never
                );
        }
    }
}

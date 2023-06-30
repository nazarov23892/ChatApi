using ChatApi.BLL.Entities;
using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Users.Concrete;
using ChatApi.BLL.Services.Users.DTOs;
using Moq;

namespace ChatApi.Tests
{
    public class UserService_CreateUser_Tests
    {
        [Fact]
        public void Cannot_Create_User_When_Username_Is_Empty()
        {
            // arrange

            Mock<IUserRepository> mock = new Mock<IUserRepository>();

            UserService target = new UserService(mock.Object);

            // act

            CreateUserResponseDto? result = target.CreateUser(new CreateUserRequestDto
            {
                UserName = string.Empty
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "UserName field is required",
                actualString: target.ValidationProblems.Keys.Single());
            mock.Verify(
                expression: m => m.Add(It.IsAny<User>()),
                times: Times.Never);
        }

        [Fact]
        public void Cannot_Create_User_When_Username_Is_Less_Than_3()
        {
            // arrange

            Mock<IUserRepository> mock = new Mock<IUserRepository>();

            UserService target = new UserService(mock.Object);

            // act

            CreateUserResponseDto? result = target.CreateUser(new CreateUserRequestDto
            {
                UserName = "ab"
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "UserName must be",
                actualString: target.ValidationProblems.Keys.Single());
            Assert.Contains(
                expectedSubstring: "minimum length of 3",
                actualString: target.ValidationProblems.Keys.Single());
            mock.Verify(
                expression: m => m.Add(It.IsAny<User>()),
                times: Times.Never);
        }

        [Fact]
        public void Cannot_Create_User_When_Username_Is_Longer_Than_20()
        {
            // arrange

            Mock<IUserRepository> mock = new Mock<IUserRepository>();

            UserService target = new UserService(mock.Object);

            // act

            CreateUserResponseDto? result = target.CreateUser(new CreateUserRequestDto
            {
                UserName = "aaaaaaaaaaaaaaaaaaaaa"
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "UserName must be",
                actualString: target.ValidationProblems.Keys.Single());
            Assert.Contains(
                expectedSubstring: "maximum length of 20",
                actualString: target.ValidationProblems.Keys.Single());
            mock.Verify(
                expression: m => m.Add(It.IsAny<User>()),
                times: Times.Never);
        }

        [Fact]
        public void Cannot_Create_User_When_Username_Contains_Symbol()
        {
            // arrange

            Mock<IUserRepository> mock = new Mock<IUserRepository>();

            CreateUserRequestDto[] requests = new[]
            {
                new CreateUserRequestDto{UserName = " ab"}, // starts with symbol
                new CreateUserRequestDto{UserName = "a-b"}, // contains symbol
                new CreateUserRequestDto{UserName = "ab-"}, // ends with symbol
                new CreateUserRequestDto{UserName = "1abc"}, // starts with number
                new CreateUserRequestDto{UserName = "***"},
            };

            foreach (var request in requests)
            {
                UserService target = new UserService(mock.Object);

                // act

                CreateUserResponseDto? result = target.CreateUser(request);

                // assert

                Assert.Null(result);
                Assert.True(target.HasValidationProblems);
                Assert.Single(target.ValidationProblems);
                Assert.Contains(
                    expectedSubstring: "Name must start with a letter and can only contain letters and numbers",
                    actualString: target.ValidationProblems.Keys.Single());
                mock.Verify(
                    expression: m => m.Add(It.IsAny<User>()),
                    times: Times.Never);
            }
        }

        [Fact]
        public void Cannot_Create_User_When_Username_Is_Exist()
        {
            // arrange

            Mock<IUserRepository> mock = new Mock<IUserRepository>();
            mock.Setup(expression: m => m.FindByName(It.IsAny<string>()))
                .Returns<string>(
                    valueFunction: s => 
                        s.Equals("username1", comparisonType: StringComparison.OrdinalIgnoreCase) 
                        ? new User 
                        { 
                            UserName = "username",
                            UserId = "111"
                        } 
                        : null);

            UserService target = new UserService(mock.Object);

            // act

            CreateUserResponseDto? result = target.CreateUser(new CreateUserRequestDto
            {
                UserName = "username1"
            });

            // assert

            Assert.Null(result);
            Assert.True(target.HasValidationProblems);
            Assert.Single(target.ValidationProblems);
            Assert.Contains(
                expectedSubstring: "the same name already exists",
                actualString: target.ValidationProblems.Keys.Single());
            mock.Verify(
                expression: m => m.Add(It.IsAny<User>()),
                times: Times.Never);
        }

        [Fact]
        public void Can_Create_User()
        {
            // arrange

            Mock<IUserRepository> mock = new Mock<IUserRepository>();
            mock.Setup(expression: m => m.FindByName(It.IsAny<string>()))
                .Returns<string>(
                    valueFunction: s =>
                        s.Equals("username1", comparisonType: StringComparison.OrdinalIgnoreCase)
                        ? new User
                        {
                            UserName = "username",
                            UserId = "111"
                        }
                        : null);

            CreateUserRequestDto[] requests = new[]
            {
                new CreateUserRequestDto{UserName = "abc"},                     // length = 3
                new CreateUserRequestDto{UserName = "abc1"},                    // contains number
                new CreateUserRequestDto{UserName = "aaaaaaaaaaaaaaaaaaa1"},    // length = 20
                new CreateUserRequestDto{UserName = "a123"},
            };

            foreach (var request in requests)
            {
                
                UserService target = new UserService(mock.Object);

                // act

                DateTime dateStart = DateTime.UtcNow;
                CreateUserResponseDto? result = target.CreateUser(request);
                DateTime dateEnd = dateStart.AddMilliseconds(50);

                // assert

                Assert.NotNull(result);
                Assert.False(target.HasValidationProblems);
                Assert.Empty(target.ValidationProblems);
                Assert.NotEmpty(result.UserId);
                mock.Verify(
                    expression: m => m.Add(It.Is<User>(u => !string.IsNullOrEmpty(u.UserId)
                        && u.UserId.Length > 0
                        && (u.CreatedAt >= dateStart && u.CreatedAt <= dateEnd)
                        )),
                    times: Times.Once);
            }
        }
    }
}
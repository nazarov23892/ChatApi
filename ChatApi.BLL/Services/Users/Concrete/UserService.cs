using ChatApi.BLL.Entities;
using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Users.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Users.Concrete
{
    public class UserService : IUserService
    {
        private List<ValidationResult> _validationProblems = new List<ValidationResult>();
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public bool HasValidationProblems
        {
            get => _validationProblems.Any();
        }

        public Dictionary<string, string[]> ValidationProblems
        {
            get => _validationProblems
               .SelectMany(
                   collectionSelector: l => l.MemberNames,
                   resultSelector: (errorMessage, memberName) => new { errorMessage = errorMessage.ErrorMessage ?? string.Empty, memberName })
               .GroupBy(
                   keySelector: e => e.errorMessage,
                   elementSelector: e => e.memberName)
               .ToDictionary(
                    keySelector: g => g.Key,
                    elementSelector: g => g.ToArray());
        }

        public CreateUserResponseDto? CreateUser(CreateUserRequestDto createUserDto)
        {
            if (!ValidateObject(createUserDto))
            {
                return null;
            }
            User user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UserName = createUserDto.UserName
            };
            _userRepository.Add(user);
            return new CreateUserResponseDto { UserId = user.UserId };
        }

        private bool ValidateObject(object instance)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(
                instance: instance,
                serviceProvider: null,
                items: null);
            bool res = Validator.TryValidateObject(
                instance: instance,
                validationContext: context,
                validationResults: results,
                validateAllProperties: true);
            if (results.Any())
            {
                _validationProblems.AddRange(results);
            }
            return res;
        }
    }
}

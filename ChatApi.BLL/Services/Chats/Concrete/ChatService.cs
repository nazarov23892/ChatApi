using ChatApi.BLL.Repositories;
using ChatApi.BLL.Services.Chats.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Chats.Concrete
{
    public class ChatService : IChatService
    {
        private readonly List<ValidationResult> _validationProblems = new List<ValidationResult>();
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
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

        public CreateChatResponseDto? Create(CreateChatRequestDto createRequestDto)
        {
            if (!ValidateObject(createRequestDto))
            {
                return null;
            }
            var existChat = _chatRepository.FindByName(createRequestDto.Name);
            if (existChat != null)
            {
                AddValidationError(
                    errorMessage: "chat with the same name already exists",
                    memberName: nameof(createRequestDto.Name));
                return null;
            }
            Entities.Chat chat = new Entities.Chat
            {
                ChatId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Name = createRequestDto.Name
            };
            _chatRepository.Add(chat);
            return new CreateChatResponseDto
            {
                ChatId = chat.ChatId
            };
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

        private void AddValidationError(string errorMessage, string? memberName = null)
        {
            _validationProblems.Add(
                new ValidationResult(
                    errorMessage: errorMessage,
                    memberNames: !string.IsNullOrEmpty(memberName)
                        ? new[] { memberName }
                        : null));
        }
    }
}

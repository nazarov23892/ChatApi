using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Basic
{
    public class BaseValidation: IValidatableService
    {
        protected List<ValidationResult> _validationProblems = new List<ValidationResult>();

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

        protected bool ValidateObject(object instance)
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

        protected void AddValidationError(string errorMessage, string? memberName = null)
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

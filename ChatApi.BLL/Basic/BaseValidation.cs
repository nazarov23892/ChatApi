using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Basic
{
    public class BaseValidation : IValidatableService
    {
        protected List<ValidationResult> _validationProblems = new List<ValidationResult>();

        public bool HasValidationProblems
        {
            get => _validationProblems.Any();
        }

        public IImmutableList<string> ValidationProblems
        {
            get => _validationProblems
                .Select(vr => vr.ErrorMessage ?? string.Empty)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToImmutableList();
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

using System;
using System.Linq;
using Conreign.Contracts.Errors;
using Conreign.Contracts.Errors.Validation;
using FluentValidation;
using FluentValidation.Results;
using ValidationFailure = Conreign.Contracts.Errors.Validation.ValidationFailure;

namespace Conreign.Core.Utility
{
    public static class ValidationExtensions
    {
        public static void EnsureIsValid<T>(this IValidator<T> validator, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }
            var results = validator.Validate(value);
            if (results.IsValid)
            {
                return;
            }
            var messages = results.Errors.Select(x => $"[{x.PropertyName}:{x.ErrorCode}] {x.ErrorMessage}");
            var message = string.Join(Environment.NewLine, messages);
            throw UserException.Create(ValidationError.BadInput, results.ToConreignValidationErrorDetails(), message);
        }

        public static void EnsureIsValid<T, TValidator>(this T value) where TValidator : IValidator<T>, new()
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var validator = new TValidator();
            validator.EnsureIsValid(value);
        }

        private static ValidationErrorDetails ToConreignValidationErrorDetails(this ValidationResult result)
        {
            if (result.IsValid)
            {
                throw new InvalidOperationException("Validation result was valid");
            }
            var failures = result.Errors.Select(x => x.ToConreignValidationFailure()).ToArray();
            return new ValidationErrorDetails(failures);
        }

        private static ValidationFailure ToConreignValidationFailure(
            this FluentValidation.Results.ValidationFailure failure)
        {
            if (failure == null)
            {
                throw new ArgumentNullException(nameof(failure));
            }
            return new ValidationFailure(failure.PropertyName, failure.ErrorCode, failure.ErrorMessage);
        }
    }
}
using System;
using System.Linq;
using Conreign.Core.Contracts.Client.Exceptions;
using Conreign.Core.Contracts.Validation;
using FluentValidation;
using FluentValidation.Results;
using ValidationFailure = Conreign.Core.Contracts.Validation.ValidationFailure;

namespace Conreign.Core.Utility
{
    internal static class ValidationExtensions
    {
        public static void EnsureIsValid<T>(this T value, IValidator<T> validator)
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
            throw UserException.Create(ValidationError.BadInput, results.ToConreignValidationErrorDetails());
        }

        public static void EnsureIsValid<T, TValidator>(this T value) where TValidator : IValidator<T>, new()
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var validator = new TValidator();
            value.EnsureIsValid(validator);
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
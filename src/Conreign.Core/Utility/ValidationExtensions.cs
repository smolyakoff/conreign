using System;
using System.Linq;
using Conreign.Core.Contracts.Common;
using Conreign.Core.Contracts.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using ValidationFailure = Conreign.Core.Contracts.Common.ValidationFailure;

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
            throw UserException.Create<ValidationError, ValidationErrorDetails>(ValidationError.BadInput, results.ToConreignValidationErrorDetails());
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

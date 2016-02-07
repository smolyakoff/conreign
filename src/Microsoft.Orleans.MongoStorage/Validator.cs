using System;
using System.Linq;

namespace Microsoft.Orleans.Storage
{
    internal class Validator
    {
        public static T EnsureIsValid<T>(T value, string name, params Func<T, string>[] validations)
        {
            var validator = Combine(validations);
            var message = validator(value);
            if (string.IsNullOrEmpty(message))
            {
                return value;
            }
            throw new ArgumentException($"Invalid MongoProvider configuration. {name} is invalid: {message}.");
        }

        private static Func<T, string> Combine<T>(params Func<T, string>[] validations)
        {
            return x => validations.Select(v => v(x)).FirstOrDefault(m => !string.IsNullOrEmpty(m));
        }
    }
}
using System;
using Orleans.Concurrency;

namespace Conreign.Contracts.Errors.Validation
{
    [Serializable]
    [Immutable]
    public class ValidationFailure
    {
        public ValidationFailure(string key, string rule, string message)
        {
            if (string.IsNullOrEmpty(rule))
            {
                throw new ArgumentException("Rule cannot be null or empty.", nameof(rule));
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            }
            Key = key;
            Rule = rule;
            Message = message;
        }

        public string Key { get; }
        public string Rule { get; }
        public string Message { get; }

        public override string ToString()
        {
            return $"{Key ?? "$"} ({Rule}): {Message}";
        }
    }
}
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Conreign.Core.Contracts.Abstractions.Data
{
    public class TypedMessage
    {
        public TypedMessage(string text, string type)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Message text should not be empty.", nameof(text));
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Message type should not be empty.", nameof(type));
            }
            Text = text;
            Type = type;
        }

        public string Type { get; set; }

        public string Text { get; set; }

        public static TypedMessage Create(Expression<Func<string>> messageGetter)
        {
            if (messageGetter == null)
            {
                throw new ArgumentNullException(nameof(messageGetter));
            }
            var messageMemberExpression = messageGetter.Body as MemberExpression;
            if (messageMemberExpression == null)
            {
                throw new ArgumentException("Message getter should be a member expression.", nameof(messageGetter));
            }
            var message = messageGetter.Compile()();
            var type = InferType(messageMemberExpression.Member.Name);
            return new TypedMessage(message, type);
        }

        private static string InferType(string memberName)
        {
            var regex = new Regex("Message$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.Replace(memberName, string.Empty);
        }
    }
}
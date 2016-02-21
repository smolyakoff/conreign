﻿using System;
using System.Linq.Expressions;

namespace Conreign.Core.Contracts.Abstractions
{
    public class UserMessage : TypedMessage
    {
        public UserMessage(string text, string type, string title = null) : base(text, type)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Message text should not be empty.", nameof(text));
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Message type should not be empty.", nameof(type));
            }
            Title = title;
        }

        public static UserMessage FromResource(Expression<Func<string>> messageGetter, Expression<Func<string>> titleGetter = null)
        {
            if (messageGetter == null)
            {
                throw new ArgumentNullException(nameof(messageGetter));
            }
            var message = TypedMessage.Create(messageGetter);
            var title = titleGetter == null ? null : TypedMessage.Create(titleGetter);
            return new UserMessage(message.Text, message.Type, title?.Text);
        }

        public string Title { get; }
    }
}

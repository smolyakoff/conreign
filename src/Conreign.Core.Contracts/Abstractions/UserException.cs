using System;
using System.Runtime.Serialization;

namespace Conreign.Core.Contracts.Abstractions
{
    [Serializable]
    public class UserException : Exception
    {
        private readonly Lazy<UserMessage> _userMessageLazy;

        public UserException(string message) : this(message, null, (string)null)
        {
        }

        public UserException(string message, Exception inner) : this(message, null, null, inner)
        {
        }

        public UserException(string message, string type) : this(message, type, (string)null)
        {
        }

        public UserException(string message, string type, Exception inner) : this(message, type, null, inner)
        {
        }

        public UserException(UserMessage message) : base(message?.Text)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            _userMessageLazy = new Lazy<UserMessage>(() => message);
            Type = message.Type;
            Title = message.Title;
        }

        public UserException(UserMessage message, Exception inner) : base(message?.Text, inner)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            _userMessageLazy = new Lazy<UserMessage>(() => message);
            Type = message.Type;
            Title = message.Title;
        }

        public UserException(string message, string type, string title) : base(message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message should not be empty.", nameof(message));
            }
            _userMessageLazy = new Lazy<UserMessage>(GetUserMessage);
            Type = string.IsNullOrEmpty(type) ? GetType().Name : type;
            Title = title;
        }

        public UserException(string message, string type, string title, Exception inner) : base(message, inner)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message should not be empty.", nameof(message));
            }
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }
            _userMessageLazy = new Lazy<UserMessage>(GetUserMessage);
            Type = string.IsNullOrEmpty(type) ? GetType().Name : type;
            Title = title;
        }

        public string Title { get; }

        public string Type { get; }

        public UserMessage UserMessage => _userMessageLazy.Value;

        protected UserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            Title = info.GetString("Title");
            Type = info.GetString("Type");
        }

        private UserMessage GetUserMessage()
        {
            return new UserMessage(Message, Type, Title);
        }
    }

    [Serializable]
    public class UserException<T> : UserException
    {
        public UserException(string message, T info = default(T)) : this(message, null, (string)null, info)
        {
        }

        public UserException(string message, Exception inner, T info = default(T)) : this(message, null, null, inner, info)
        {
        }

        public UserException(string message, string type, T info = default(T)) : this(message, type, (string)null, info)
        {
        }

        public UserException(string message, string type, Exception inner, T info = default(T)) : this(message, type, null, inner, info)
        {
        }

        public UserException(UserMessage message, T info = default(T)) : base(message)
        {
            Info = info;
        }

        public UserException(UserMessage message, Exception inner, T info = default(T)) : base(message, inner)
        {
            Info = info;
        }

        public UserException(string message, string type, string title, T info = default(T)) : base(message, type, title)
        {
            Info = info;
        }

        public UserException(string message, string type, string title, Exception inner, T info = default(T)) 
            : base(message, type, title, inner)
        {
            Info = info;
        }

        public T Info { get; }

        protected UserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            Info = (T)info.GetValue("Info", typeof (T));
        }
    }
}

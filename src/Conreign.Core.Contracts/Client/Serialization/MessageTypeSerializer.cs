using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Communication;
using MediatR;

namespace Conreign.Core.Contracts.Client.Serialization
{
    public static class MessageTypeSerializer
    {
        private static readonly Dictionary<string, Type> Types;

        static MessageTypeSerializer()
        {
            var assemblies = new[] {typeof(LoginCommand).Assembly, typeof(IClientEvent).Assembly};
            var exportedTypes = assemblies.SelectMany(x => x.GetExportedTypes()).ToList();
            Types = exportedTypes
                .Where(t => !t.IsAbstract)
                .Where(t => IsCommand(t) || IsEvent(t))
                .SelectMany(t => GetMessageTypes(t, exportedTypes))
                .Distinct()
                .ToDictionary(Serialize, t => t, StringComparer.OrdinalIgnoreCase);
        }

        public static string Serialize(Type type)
        {
            return type.Name.TrimPostfix("Command");
        }

        public static Type Deserialize(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Type cannot be null or empty.", nameof(type));
            }
            if (!Types.ContainsKey(type))
            {
                throw new NotSupportedException($"Command or event type {type} is not supported.");
            }
            return Types[type];
        }

        private static string TrimPostfix(this string s, string postfix)
        {
            return s.EndsWith(postfix) ? s.Substring(0, s.Length - postfix.Length) : s;
        }

        private static bool IsCommand(Type type)
        {
            var @interface = type
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncRequest<>));
            return @interface != null;
        }

        private static bool IsEvent(Type type)
        {
            return typeof(IClientEvent).IsAssignableFrom(type) && type.IsClass;
        }

        private static IEnumerable<Type> GetMessageTypes(Type type, IEnumerable<Type> types)
        {
            if (IsCommand(type))
            {
                // Return response types for commands
                var @interface = type
                    .GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncRequest<>));
                var responseType = @interface.GetGenericArguments()[0];
                if (responseType.IsInterface || responseType.IsAbstract)
                {
                    foreach (var implementationType in types.Where(t => responseType.IsAssignableFrom(t)))
                    {
                        yield return implementationType;
                    }
                }
                else
                {
                    yield return responseType;
                }
            }
            yield return type;
        }
    }
}
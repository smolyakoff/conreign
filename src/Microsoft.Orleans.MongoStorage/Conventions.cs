using System;
using System.Linq;
using System.Text.RegularExpressions;
using Orleans.Runtime;

namespace Microsoft.Orleans.Storage
{
    internal static class Conventions
    {
        public static string CollectionNameForGrain(string grainType, string prefix)
        {
            if (string.IsNullOrEmpty(grainType))
            {
                throw new ArgumentNullException(nameof(grainType), "Grain type should not be empty.");
            }
            var names = grainType.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);
            var collectionName = names.Last()
                .WithoutGrainPostfix()
                .ToSnakeCase()
                .WithPrefix(prefix ?? names.First());
            return collectionName;
        }

        public static string PrimaryKeyForGrain(Guid serviceId, GrainReference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference), "Grain reference should not be null.");
            }
            var key = reference.ToKeyString();
            var serviceIdHash = CalculateShortHash(serviceId.ToByteArray());
            return $"{serviceIdHash}-{key}";
        }

        private static string CalculateShortHash(byte[] bytes)
        {
            var hash = xxHash.CalculateHash(bytes);
            var hashString = Convert.ToBase64String(BitConverter.GetBytes(hash))
                .Replace("=", string.Empty);
            return hashString;
        }

        private static string WithPrefix(this string name, string prefix)
        {
            return $"{prefix.ToLowerInvariant()}_{name}";
        }

        private static string WithoutGrainPostfix(this string name)
        {
            var regex = new Regex("grain$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.Replace(name, string.Empty);
        }

        private static string ToSnakeCase(this string name)
        {
            var symbols = name.SelectMany((c, i) =>
            {
                if (char.IsLower(c))
                {
                    return new[] {c};
                }
                if (i > 0 && char.IsLower(name[i - 1]))
                {
                    return new[] {'_', char.ToLowerInvariant(c)};
                }
                return new[] {char.ToLowerInvariant(c)};
            });
            return new string(symbols.ToArray());
        }
    }
}
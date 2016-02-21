using System;

namespace Conreign.Core.Contracts.Abstractions
{
    public class ActionAttribute : Attribute
    {
        public ActionAttribute()
        {
        }

        public ActionAttribute(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException(nameof(type));
            }
            Type = type;
        }

        public string Type { get; private set; }
    }
}

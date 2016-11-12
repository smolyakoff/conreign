using System;

namespace Conreign.LoadTest.Supervisor.Utility
{
    public class CommandLineArgument
    {
        public CommandLineArgument(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }

        public override string ToString()
        {
            return Value == null ? string.Empty : $"--{Name}=\"{Value}\"";
        }
    }
}
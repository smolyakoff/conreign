namespace Conreign.Core.Contracts.Abstractions.Data
{
    public class KeyPayload<T>
    {
        public KeyPayload(T key)
        {
            Key = key;
        }

        public T Key { get; set; }
    }
}
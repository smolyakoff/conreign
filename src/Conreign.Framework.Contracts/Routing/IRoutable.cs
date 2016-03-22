namespace Conreign.Framework.Contracts.Routing
{
    public interface IRoutable<out TKey>
    {
        TKey Key { get; }
    }
}
namespace Conreign.Framework.Routing
{
    public interface IRoute
    {
        string Key { get; }

        RouteType Type { get; }
    }
}
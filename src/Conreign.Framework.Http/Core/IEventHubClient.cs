namespace Conreign.Framework.Http.Core
{
    public interface IEventHubClient
    {
        void Receive(object @event);
    }
}
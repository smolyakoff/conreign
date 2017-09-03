namespace Conreign.Client.Orleans
{
    public interface IOrleansClientInitializer
    {
        void Initialize();
        void Uninitialize();
    }
}
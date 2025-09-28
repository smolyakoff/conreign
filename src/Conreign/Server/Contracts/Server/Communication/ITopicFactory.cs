namespace Conreign.Server.Contracts.Server.Communication;

public interface ITopicFactory
{
    Task<ITopic> Create(string id);
}
namespace Conreign.Core.Contracts.Abstractions
{
    public interface IPayloadContainer<out TPayload>
    {
        TPayload Payload { get; }
    }
}
namespace Conreign.Framework.Contracts.Core.Data
{
    public interface IPayloadContainer<out TPayload>
    {
        TPayload Payload { get; }
    }
}
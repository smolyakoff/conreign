namespace Conreign.Core.Contracts.Abstractions
{
    public interface IMetadataContainer<out TMeta>
    {
        TMeta Meta { get; }
    }
}
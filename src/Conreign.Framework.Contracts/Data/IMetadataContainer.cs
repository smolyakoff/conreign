namespace Conreign.Framework.Contracts.Core.Data
{
    public interface IMetadataContainer<out TMeta>
    {
        TMeta Meta { get; }
    }
}
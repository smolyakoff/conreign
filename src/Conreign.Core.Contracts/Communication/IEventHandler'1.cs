using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Conreign.Core.Contracts.Communication
{
    [BsonKnownTypes(typeof(BsonReq))]
    public interface IEventHandler<in T> where T : class 
    {
        Task Handle(T @event);
    }
}
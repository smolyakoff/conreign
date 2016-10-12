using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Conreign.Host.Storage
{
    public class TypeSerializer : SerializerBase<Type>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Type value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }
            context.Writer.WriteString(value.AssemblyQualifiedName);
        }

        public override Type Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                return null;
            }
            //if (context.Reader.CurrentBsonType != BsonType.String)
            //{
            //    throw new InvalidOperationException("Expected .NET type to be represented as string.");
            //}
            var type = context.Reader.ReadString();
            if (string.IsNullOrEmpty(type))
            {
                throw new InvalidOperationException("Expected type to be not empty.");
            }
            return Type.GetType(type);
        }
    }
}

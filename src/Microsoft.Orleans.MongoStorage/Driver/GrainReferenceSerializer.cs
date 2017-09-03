using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Orleans;
using Orleans.Runtime;

namespace Microsoft.Orleans.MongoStorage.Driver
{
    public class GrainReferenceSerializer : SerializerBase<GrainReference>
    {
        private const string InterfaceNameField = "grain_interface";
        private const string KeyField = "grain_key";

        private static readonly MethodInfo DeserializationMethod = typeof(GrainExtensions).GetMethod(
            "Cast",
            BindingFlags.Static | BindingFlags.Public);

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args,
            GrainReference value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }
            var grainReference = value;
            var grainKey = grainReference.ToKeyString();
            var grainInterfaceType = grainReference
                .GetType()
                .GetInterfaces()
                .FirstOrDefault(t => typeof(IGrain).IsAssignableFrom(t));
            if (grainInterfaceType == null)
            {
                throw new InvalidOperationException("Expected grain reference to implement IGrain.");
            }
            context.Writer.WriteStartDocument();
            context.Writer.WriteString(InterfaceNameField, grainInterfaceType.AssemblyQualifiedName);
            context.Writer.WriteString(KeyField, grainKey);
            context.Writer.WriteEndDocument();
        }

        public override GrainReference Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }
            EnsureBsonTypeEquals(context.Reader, BsonType.Document);
            // The MongoDB document will look like 
            // { "_t": "CodeGenGrainName", _v: { grain_interface: "Fully qualified type name", grain_key: "GrainReference=..."} }
            context.Reader.ReadStartDocument();
            context.Reader.FindElement("_v");
            context.Reader.ReadStartDocument();
            var interfaceName = context.Reader.ReadString(InterfaceNameField);
            var grainKey = context.Reader.ReadString(KeyField);
            context.Reader.ReadEndDocument();
            context.Reader.ReadEndDocument();
            if (string.IsNullOrEmpty(interfaceName) || string.IsNullOrEmpty(grainKey))
            {
                throw new InvalidOperationException(
                    $"Expected ${InterfaceNameField} and ${KeyField} fields in the document. " +
                    $"Got ${InterfaceNameField}={interfaceName}, {KeyField}={grainKey}.");
            }
            var grainReference = GrainReference.FromKeyString(grainKey);
            var grainInterfaceType = LookupGrainInterfaces(interfaceName, args.NominalType)
                .FirstOrDefault(x => x != null);
            if (grainInterfaceType == null)
            {
                throw new InvalidOperationException(
                    $"Failed to resolve grain interface type. Serialized type was: {interfaceName}.");
            }
            var deserialize = DeserializationMethod.MakeGenericMethod(grainInterfaceType);
            var grainInterface = deserialize.Invoke(null, new object[] {grainReference});
            return (GrainReference) grainInterface;
        }

        private static IEnumerable<Type> LookupGrainInterfaces(string serializedName, Type nominalType)
        {
            var choices = new List<Type> {nominalType};
            choices.AddRange(nominalType.GetInterfaces());
            choices.RemoveAll(x => !typeof(IGrain).GetTypeInfo().IsAssignableFrom(x));
            foreach (var choice in choices)
                yield return choice;
            yield return Type.GetType(serializedName, false);
        }
    }
}
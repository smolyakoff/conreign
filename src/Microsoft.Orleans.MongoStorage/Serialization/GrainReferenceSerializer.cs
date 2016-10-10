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

namespace Microsoft.Orleans.Storage.Serialization
{
    public class GrainReferenceSerializer : SerializerBase<GrainReference>
    {
        private const string InterfaceNameField = "_t";
        private const string KeyField = "key";

        private static readonly MethodInfo DeserializationMethod = typeof(GrainExtensions).GetMethod(
            "Cast",
            BindingFlags.Static | BindingFlags.Public);

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, GrainReference value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }
            var grainReference = value;
            var grainKey = grainReference.ToKeyString();
            context.Writer.WriteStartDocument();
            context.Writer.WriteString(InterfaceNameField, grainReference.InterfaceName.Replace("global::", string.Empty));
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
            if (context.Reader.CurrentBsonType != BsonType.Document)
            {
                throw new InvalidOperationException("Expected a document to deserialize a grain reference.");
            }
            context.Reader.ReadStartDocument();
            context.Reader.FindElement("_v");
            context.Reader.ReadStartDocument();
            var interfaceName = context.Reader.ReadString(InterfaceNameField);
            var grainKey = context.Reader.ReadString(KeyField);
            context.Reader.ReadEndDocument();
            context.Reader.ReadEndDocument();
            if (string.IsNullOrEmpty(interfaceName) || string.IsNullOrEmpty(grainKey))
            {
                throw new InvalidOperationException($"Expected _t and key fields in the document. Got _t={interfaceName}, key={grainKey}.");
            }
            var grainReference = GrainReference.FromKeyString(grainKey);
            var grainInterfaceType = LookupGrainInterfaces(interfaceName, args.NominalType)
                .FirstOrDefault(x => x != null);
            if (grainInterfaceType == null)
            {
                throw new InvalidOperationException($"Failed to resolve grain interface type. Serialized type was: {interfaceName}.");
            }
            var deserialize = DeserializationMethod.MakeGenericMethod(grainInterfaceType);
            var grainInterface = deserialize.Invoke(null, new []{grainReference});
            return (GrainReference)grainInterface;
        }

        private static IEnumerable<Type> LookupGrainInterfaces(string serializedName, Type nominalType)
        {
            var choices = new List<Type> {nominalType};
            choices.AddRange(nominalType.GetInterfaces());
            choices.RemoveAll(x => !typeof(IGrain).GetTypeInfo().IsAssignableFrom(x));
            foreach (var choice in choices)
            {
                yield return choice;
            }
            // TODO: performance
            var type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetExportedTypes())
                .FirstOrDefault(x => x.FullName == serializedName);
            yield return type;
        }
    }
}

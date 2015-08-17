﻿namespace NServiceBus
{
    using System;
    using System.IO;
    using System.Linq;
    using NServiceBus.OutgoingPipeline;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Serialization;
    using NServiceBus.Settings;
    using NServiceBus.TransportDispatch;
    using NServiceBus.Unicast.Messages;

    class SerializeMessagesBehavior : StageConnector<OutgoingContext, PhysicalOutgoingContextStageBehavior.Context>
    {
        readonly IMessageSerializer messageSerializer;
        readonly MessageMetadataRegistry messageMetadataRegistry;
        readonly ReadOnlySettings settings;

        public SerializeMessagesBehavior(IMessageSerializer messageSerializer, MessageMetadataRegistry messageMetadataRegistry, ReadOnlySettings settings)
        {
            this.messageSerializer = messageSerializer;
            this.messageMetadataRegistry = messageMetadataRegistry;
            this.settings = settings;
        }

        public override void Invoke(OutgoingContext context, Action<PhysicalOutgoingContextStageBehavior.Context> next)
        {
            if (context.GetOrCreate<State>().SkipSerialization)
            {
                next(new PhysicalOutgoingContextStageBehavior.Context(new byte[0], context));
                return;
            }

            using (var ms = new MemoryStream())
            {
                messageSerializer.Serialize(context.GetMessageInstance(), ms);

                var serializerDefinition = settings.GetSelectedSerializer();
                context.SetHeader(Headers.ContentType, serializerDefinition.ContentType);

                context.SetHeader(Headers.EnclosedMessageTypes, SerializeEnclosedMessageTypes(context.GetMessageType()));
                next(new PhysicalOutgoingContextStageBehavior.Context(ms.ToArray(), context));
            }
        }

        string SerializeEnclosedMessageTypes(Type messageType)
        {
            var metadata = messageMetadataRegistry.GetMessageMetadata(messageType);
            var distinctTypes = metadata.MessageHierarchy.Distinct();

            return string.Join(";", distinctTypes.Select(t => t.AssemblyQualifiedName));
        }

        public class State
        {
            public bool SkipSerialization { get; set; }
        }

    }

    /// <summary>
    /// Allows users to control serialization.
    /// </summary>
    public static class SerializationContextExtensions
    {
        /// <summary>
        /// Requests the serializer to skip serializing the message.
        /// </summary>
        public static void SkipSerialization(this OutgoingContext context)
        {
            context.GetOrCreate<SerializeMessagesBehavior.State>().SkipSerialization = true;
        }
    }
}
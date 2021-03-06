﻿namespace NServiceBus.Core.Tests.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NServiceBus.Serialization;
    using NServiceBus.Unicast.Messages;
    using NUnit.Framework;
    using Conventions = NServiceBus.Conventions;

    public class SerializeMessagesBehaviorTests
    {
        [Test]
        public void Should_set_content_type_header()
        {
            var registry = new MessageMetadataRegistry(new Conventions());

            registry.RegisterMessageType(typeof(MyMessage));

            var behavior = new SerializeMessagesBehavior(new FakeSerializer("myContentType"),registry);

            var context = ContextHelpers.GetOutgoingContext(new MyMessage());
            behavior.Invoke(context, c =>
            {
                
            });

            Assert.AreEqual("myContentType", context.GetOrCreate<DispatchMessageToTransportConnector.State>().Headers[Headers.ContentType]);
        }

        public class FakeSerializer : IMessageSerializer
        {
            public FakeSerializer(string contentType)
            {
                ContentType = contentType;
            }

            public void Serialize(object message, Stream stream)
            {
                
            }

            public object[] Deserialize(Stream stream, IList<Type> messageTypes = null)
            {
                throw new NotImplementedException();
            }

            public string ContentType { get; private set; }
        }

        class MyMessage { }
    }
}
﻿namespace NServiceBus.AcceptanceTests.Basic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using NServiceBus.AcceptanceTesting;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NServiceBus.Features;
    using NServiceBus.Serialization;
    using NUnit.Framework;

    public class When_registering_additional_deserializers : NServiceBusAcceptanceTest
    {
        [Test]
        public void Should_compose_with_default_serializer()
        {
            var context = new Context();

            Scenario.Define(context)
                .WithEndpoint<CustomSerializationSender>(b => b.Given(
                    (bus, c) =>
                    {
                        var sendOptions = new SendOptions();

                        sendOptions.SetHeader("ContentType", "MyCustomSerializer");

                        bus.Send(new MyRequest());
                    }))
                .WithEndpoint<XmlCustomSerializationReceiver>()
                .Done(c => c.DeserializeCalled)
                .Run();
            
            Assert.True(context.DeserializeCalled);
        }

        public class Context : ScenarioContext
        {
            public bool HandlerGotTheRequest { get; set; }
            public bool SerializeCalled { get; set; }
            public bool DeserializeCalled { get; set; }
        }

        public class CustomSerializationSender : EndpointConfigurationBuilder
        {
            public CustomSerializationSender()
            {
                EndpointSetup<DefaultServer>(c => c.UseSerialization<MyCustomSerializer>())
                    .AddMapping<MyRequest>(typeof(XmlCustomSerializationReceiver));
            }
        }

        public class XmlCustomSerializationReceiver : EndpointConfigurationBuilder
        {
            public XmlCustomSerializationReceiver()
            {
                EndpointSetup<DefaultServer>(c =>
                {
                    c.UseSerialization<XmlSerializer>();
                    c.AddDeserializer<MyCustomSerializer>();
                });
            }

            public class MyRequestHandler : IHandleMessages<MyRequest>
            {
                public Context Context { get; set; }

                public void Handle(MyRequest request)
                {
                    Context.HandlerGotTheRequest = true;
                }
            }
        }

        [Serializable]
        public class MyRequest : IMessage
        {
        }

        class MyCustomSerializer : SerializationDefinition
        {
            protected override Type ProvidedByFeature()
            {
                return typeof(MyCustomSerialization);
            }
        }

        class MyCustomSerialization : ConfigureSerialization
        {
            public MyCustomSerialization()
            {
                EnableByDefault();
            }

            protected override Type GetSerializerType(FeatureConfigurationContext context)
            {
                return typeof(MyCustomMessageSerializer);
            }
        }

        class MyCustomMessageSerializer : IMessageSerializer
        {
            public Context Context { get; set; }

            public void Serialize(object message, Stream stream)
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(stream, message);

                Context.SerializeCalled = true;
            }

            public object[] Deserialize(Stream stream, IList<Type> messageTypes = null)
            {
                var serializer = new BinaryFormatter();

                Context.DeserializeCalled = true;
                stream.Position = 0;
                var msg = serializer.Deserialize(stream);

                return new[]
                {
                    msg
                };
            }

            public string ContentType
            {
                get { return "MyCustomSerializer"; }
            }
        }
    }
}
﻿namespace NServiceBus.AcceptanceTests.Routing
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.AcceptanceTesting;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NServiceBus.AcceptanceTests.ScenarioDescriptors;
    using NServiceBus.Features;
    using NServiceBus.Persistence;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;
    using NUnit.Framework;

    public class When_publishing_from_sendonly : NServiceBusAcceptanceTest
    {
        [Test]
        public void Should_be_delivered_to_all_subscribers()
        {
            var context = new Context();

            Scenario.Define(context)
                .WithEndpoint<SendOnlyPublisher>(b => b.Given((bus, c) => bus.Publish(new MyEvent())))
                .WithEndpoint<Subscriber>()
                .Done(c => c.SubscriberGotTheEvent)
                .Repeat(r => r.For<AllTransportsWithMessageDrivenPubSub>())
                .Should(ctx => Assert.True(ctx.SubscriberGotTheEvent))
                .Run();
        }

        public class Context : ScenarioContext
        {
            public bool SubscriberGotTheEvent { get; set; }
        }

        public class SendOnlyPublisher : EndpointConfigurationBuilder
        {
            public SendOnlyPublisher()
            {
                EndpointSetup<DefaultPublisher>(b =>
                {
                    b.UsePersistence(typeof(HardCodedPersistence));
                    b.DisableFeature<AutoSubscribe>();
                }).SendOnly();
            }
        }

        public class Subscriber : EndpointConfigurationBuilder
        {
            public Subscriber()
            {
                EndpointSetup<DefaultServer>(c => c.DisableFeature<AutoSubscribe>())
                    .AddMapping<MyEvent>(typeof(SendOnlyPublisher));
            }

            public class MyEventHandler : IHandleMessages<MyEvent>
            {
                public Context Context { get; set; }

                public void Handle(MyEvent messageThatIsEnlisted)
                {
                    Context.SubscriberGotTheEvent = true;
                }
            }
        }

        [Serializable]
        public class MyEvent : IEvent
        {
        }

        public class HardCodedPersistence : PersistenceDefinition
        {
            internal HardCodedPersistence()
            {
                Supports<StorageType.Subscriptions>(s => s.EnableFeatureByDefault<HardCodedPersistenceFeature>());
            }
        }

        public class HardCodedPersistenceFeature:Feature
        {
            protected override void Setup(FeatureConfigurationContext context)
            {
                context.Container.ConfigureComponent<HardcodedSubscriptionQuery>(DependencyLifecycle.SingleInstance);
                context.Container.ConfigureComponent<NoOpSubscriptionManager>(DependencyLifecycle.SingleInstance);
            }
        }

        public class HardcodedSubscriptionQuery : IQuerySubscriptions
        {
            public IEnumerable<string> GetSubscriberAddressesForMessage(IEnumerable<MessageType> messageTypes)
            {
                return new[]
                {
                    "publishingfromsendonly.subscriber"
                };
            }
        }

        public class NoOpSubscriptionManager : ISubscriptionStorage
        {
            public void Subscribe(string client, IEnumerable<MessageType> messageTypes, SubscriptionStorageOptions options)
            {
            }

            public void Unsubscribe(string client, IEnumerable<MessageType> messageTypes, SubscriptionStorageOptions options)
            {
            }
        }
    }
}
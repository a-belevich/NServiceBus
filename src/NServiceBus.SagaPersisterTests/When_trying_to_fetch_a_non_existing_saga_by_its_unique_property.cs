﻿using System;
using NServiceBus.Saga;
using NUnit.Framework;

namespace NServiceBus.SagaPersisterTests
{
    [TestFixture]
    public class When_trying_to_fetch_a_non_existing_saga_by_its_unique_property
    {
        [Test]
        public void It_should_return_null()
        {
            var persisterAndSession = TestSagaPersister.ConstructPersister();
            var persister = persisterAndSession.Item1;
            var session = persisterAndSession.Item2;

            session.Begin();
            Assert.Null(persister.Get<SagaData>("UniqueString", Guid.NewGuid().ToString()));
            session.End();
        }

        public class SagaData : IContainSagaData
        {
            public Guid Id { get; set; }
            public string Originator { get; set; }
            public string OriginalMessageId { get; set; }
            [Unique]
            public string UniqueString { get; set; }
        }
    }
}
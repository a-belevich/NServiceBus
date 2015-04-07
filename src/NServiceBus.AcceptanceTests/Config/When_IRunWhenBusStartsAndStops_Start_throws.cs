﻿namespace NServiceBus.AcceptanceTests.Config
{
    using System;
    using NServiceBus.AcceptanceTesting;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NUnit.Framework;

    public class When_Start_throws_new : NServiceBusAcceptanceTest
    {
        [Test]
        public void Should_shutdown_bus_cleanly()
        {
            Scenario.Define<Context>()
                    .WithEndpoint<StartedEndpoint>()
                    .Done(c => c.IsDone)
                    .Run();
        }

        public class Context : ScenarioContext
        {
            public bool IsDone { get; set; }
        }

        public class StartedEndpoint : EndpointConfigurationBuilder
        {
            public StartedEndpoint()
            {
                EndpointSetup<DefaultServer>();
            }

            class AfterConfigIsComplete:IRunWhenBusStartsAndStops
            {
                public Context Context { get; set; }

                public void Start(IRunContext context)
                {
                    Context.IsDone = true;

                    throw new Exception("Boom!");
                }

                public void Stop(IRunContext context)
                {
                }
            }
        }
    }


}
﻿namespace NServiceBus.Features
{
    using Serializers.Binary;

    /// <summary>
    /// Usd to configure binary as message serialization.
    /// </summary>
    public class BinarySerialization : Feature
    {
        
        internal BinarySerialization()
        {
        }
        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Container.ConfigureComponent<SimpleMessageMapper>(DependencyLifecycle.SingleInstance);
            context.Container.ConfigureComponent<BinaryMessageSerializer>(DependencyLifecycle.SingleInstance);
        }
    }
}
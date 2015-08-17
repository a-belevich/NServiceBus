﻿namespace NServiceBus.Serialization
{
    using System;
    using NServiceBus.Features;
    using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
    using NServiceBus.Serializers;

    /// <summary>
    /// Base class for configuring <see cref="SerializationDefinition"/> features.
    /// </summary>
    public abstract class ConfigureSerialization : Feature
    {
        /// <inheritdoc />
        protected ConfigureSerialization()
        {
            EnableByDefault();
            Prerequisite(this.ShouldSerializationFeatureBeEnabled, string.Format("{0} not enabled since serialization definition not detected.", GetType()));
        }

        /// <summary>
        /// Called when the features is activated.
        /// </summary>
        protected internal sealed override void Setup(FeatureConfigurationContext context)
        {
            context.Container.ConfigureComponent<MessageDeserializerResolver>(DependencyLifecycle.SingleInstance);
            context.Container.ConfigureComponent<MessageMapper>(DependencyLifecycle.SingleInstance);

            RegisterSerializer(context);
        }

        /// <summary>
        /// Specify the concrete implementation of <see cref="IMessageSerializer"/> type.
        /// </summary>
        protected abstract Type GetSerializerType(FeatureConfigurationContext context);

        /// <summary>
        /// Registeres the specified implementation of <see cref="IMessageSerializer"/>
        /// </summary>
        protected virtual void RegisterSerializer(FeatureConfigurationContext context)
        {
            var serializerType = GetSerializerType(context);
            if (serializerType == null)
            {
                return;
            }

            if (!typeof(IMessageSerializer).IsAssignableFrom(serializerType))
            {
                throw new InvalidOperationException("The type needs to implement IMessageSerializer.");
            }

            var c = context.Container.ConfigureComponent(serializerType, DependencyLifecycle.SingleInstance);
            context.Settings.ApplyTo(serializerType, c);
        }
    }
}
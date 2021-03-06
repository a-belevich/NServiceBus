﻿namespace NServiceBus.Outbox
{
    using NServiceBus.Extensibility;

    /// <summary>
    /// Provides details about the current outbox storage operation.
    /// </summary>
    public class OutboxStorageOptions
    {
        /// <summary>
        /// Creates a new instance of the OutboxStorageOptions class.
        /// </summary>
        /// <param name="context">The context.</param>
        public OutboxStorageOptions(ReadOnlyContextBag context)
        {
            Context = context;
        }

        /// <summary>
        /// Access to the behavior context.
        /// </summary>
        public ReadOnlyContextBag Context { get; set; }
    }
}
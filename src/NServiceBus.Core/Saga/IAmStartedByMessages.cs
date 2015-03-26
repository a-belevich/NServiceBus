namespace NServiceBus.Saga
{

    /// <summary>
    /// Use this interface to signify that when a message of the given type is
    /// received, if a saga cannot be found by an <see cref="IFindSagas{T}"/>
    /// the saga will be created.
    /// </summary>
    public interface IAmStartedByMessages<T> : IHandleMessages<T>
    {
    }

#pragma warning disable 1591

    public interface IAmStartedByConsumedMessage<T> : IConsumeMessage<T>
    {
    }


    public interface IAmStartedByConsumedEvent<T> : IConsumeEvent<T>
    { }
#pragma warning restore 1591
}

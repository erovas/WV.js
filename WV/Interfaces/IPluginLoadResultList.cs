namespace WV.Interfaces
{
    public interface IPluginLoadResultList : IWVList<IPluginLoadResult>
    {
        int SuccessCount { get; }

        int FailureCount { get; }

        bool AnySuccess { get; }

        bool AnyFailure { get; }

        bool AllSuccess { get; }

        bool AllFailed { get; }

        IPluginLoadResultList Successes();

        IPluginLoadResultList Failures();
    }
}
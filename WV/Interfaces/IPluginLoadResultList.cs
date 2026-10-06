namespace WV.Interfaces
{
    public interface IPluginLoadResultList
    {
        IPluginLoadResult this[int index] { get; }

        int Length { get; }

        int Count { get; }

        bool IsEmpty { get; }

        bool Any { get; }

        int SuccessCount { get; }

        int FailureCount { get; }

        bool AnySuccess { get; }

        bool AnyFailure { get; }

        bool AllSuccess { get; }

        bool AllFailed { get; }

        IPluginLoadResult? First { get; }

        IPluginLoadResult? Last { get; }

        IPluginLoadResult? Get(int index);

        IPluginLoadResultList Successes();

        IPluginLoadResultList Failures();
    }
}
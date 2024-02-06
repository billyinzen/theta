namespace Theta.Common.Services;

public class DateTimeOffsetProviderContext : IDisposable
{
    internal DateTimeOffset ContextDateTimeNow;
    private static readonly ThreadLocal<Stack<DateTimeOffsetProviderContext>> ThreadScopeStack = new(() => new Stack<DateTimeOffsetProviderContext>());

    public DateTimeOffsetProviderContext(DateTimeOffset contextDateTimeNow)
    {
        ContextDateTimeNow = contextDateTimeNow;
        ThreadScopeStack.Value?.Push(this);
    }

    public static DateTimeOffsetProviderContext? Current 
        => ThreadScopeStack.Value?.Count == 0 ? null : ThreadScopeStack.Value?.Peek();

    public void Dispose()
    {
        ThreadScopeStack.Value?.Pop();
        GC.SuppressFinalize(this);
    }
}
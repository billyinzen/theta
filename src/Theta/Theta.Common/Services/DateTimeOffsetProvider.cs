namespace Theta.Common.Services;

public static class DateTimeOffsetProvider
{
    public static DateTimeOffset Now
        => DateTimeOffsetProviderContext.Current == null
            ? DateTime.Now
            : DateTimeOffsetProviderContext.Current.ContextDateTimeNow;
}
namespace Theta.Common.Services;

public class DateTimeOffsetProvider
{
    public static DateTimeOffset Now
        => DateTimeOffsetProviderContext.Current == null
            ? DateTime.Now
            : DateTimeOffsetProviderContext.Current.ContextDateTimeNow;
}
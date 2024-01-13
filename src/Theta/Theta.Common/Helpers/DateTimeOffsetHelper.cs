namespace Theta.Common.Helpers;

/// <summary>
/// Helper class providing a DateTimeOffset wrapper
/// </summary>
public static class DateTimeOffsetHelper
{
    /// <summary>
    /// Wrapper around DateTimeOffset.Now, unless otherwise configured using <see cref="Set"/>
    /// </summary>
    public static Func<DateTimeOffset> Now = () => DateTimeOffset.Now;

    /// <summary>
    /// Overrides the default DateTimeOffset.Now response with a custom value
    /// </summary>
    /// <param name="dateTime"></param>
    public static void Set(DateTimeOffset dateTime)
        => Now = () => dateTime;

    /// <summary>
    /// Resets the Now response to DateTimeOffset.Now
    /// </summary>
    public static void Reset()
        => Now = () => DateTimeOffset.Now;
}
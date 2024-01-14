namespace Theta.Common.Models;

public class CommandResult
{
    public bool IsSuccess { get; private init; }
    
    public string? Error { get; private init; }
    
    public static implicit operator CommandResult(bool success)
        => new() { IsSuccess = success };

    public static implicit operator CommandResult(string error)
        => new() { IsSuccess = false, Error = error };
}
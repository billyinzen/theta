namespace Theta.Common.Models;

public class CommandResult
{
    public bool IsSuccess { get; set; }
    
    public string? Error { get; set; }

    public static CommandResult Success = true;

    public static implicit operator CommandResult(bool success)
        => new() { IsSuccess = true };

    public static implicit operator CommandResult(string error)
        => new() { IsSuccess = false, Error = error };
}
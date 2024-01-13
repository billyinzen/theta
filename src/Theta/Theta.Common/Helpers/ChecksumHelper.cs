namespace Theta.Common.Helpers;

public static class ChecksumHelper
{
    /// <summary>
    /// Generates a cryptographic hash value for a given string
    /// </summary>
    /// <param name="input">The string to hash</param>
    /// <returns></returns>
    public static string GetHashValue(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        
        var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}
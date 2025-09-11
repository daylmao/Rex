using System.Security.Cryptography;

namespace Rex.Application.Utilities;

/// <summary>
/// Static class for generating secure numeric codes.
/// </summary>
public static class CodeGenerator
{
    /// <summary>
    /// Generates a random numeric code with the specified number of digits.
    /// </summary>
    /// <param name="digits">The number of digits for the code (default is 6, valid range 1-9).</param>
    /// <returns>A numeric code as a string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when digits is less than 1 or greater than 9.
    /// </exception>
    public static string GenerateCode(int digits = 6)
    {
        if (digits <= 0 || digits > 9)
            throw new ArgumentOutOfRangeException(nameof(digits), "Digits must be between 1 and 9");

        int min = (int)Math.Pow(10, digits - 1);
        int max = (int)Math.Pow(10, digits);

        int number = RandomNumberGenerator.GetInt32(min, max);

        return number.ToString();
    }
}
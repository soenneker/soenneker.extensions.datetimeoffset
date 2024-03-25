using System.Diagnostics.Contracts;

namespace Soenneker.Extensions.DateTimeOffset;

/// <summary>
/// A collection of helpful DateTimeOffset extension methods
/// </summary>
public static class DateTimeOffsetExtension
{
    /// <summary>
    /// Converts the <see cref="DateTimeOffset"/> to a UTC <see cref="System.DateTime"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> to convert.</param>
    /// <returns>A UTC <see cref="System.DateTime"/> equivalent to the provided <see cref="DateTimeOffset"/>.</returns>
    [Pure]
    public static System.DateTime ToUtcDateTime(this System.DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.UtcDateTime;
    }
}
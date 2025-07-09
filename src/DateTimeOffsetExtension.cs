using System;
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
    public static DateTime ToUtcDateTime(this System.DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.UtcDateTime;
    }

    /// <summary>
    /// Determines whether the date is a business day (Mon–Fri) in the supplied time-zone.
    /// If <paramref name="zone"/> is <see langword="null"/>, the date’s own offset is used.
    /// </summary>
    [Pure]
    public static bool IsBusinessDay(this System.DateTimeOffset dateTimeOffset, TimeZoneInfo? zone = null)
    {
        System.DateTimeOffset local = zone is null ? dateTimeOffset : TimeZoneInfo.ConvertTime(dateTimeOffset, zone);
        DayOfWeek d = local.DayOfWeek;
        return d != DayOfWeek.Saturday && d != DayOfWeek.Sunday;
    }

    /// <summary>
    /// Adds (or subtracts) a number of business days, skipping weekends in the given time-zone.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="businessDays">Positive to add, negative to subtract.</param>
    /// <param name="zone">
    /// Time-zone whose calendar should be used when deciding if a day is a weekend.
    /// If <see langword="null"/>, the date’s own offset is used.
    /// </param>
    [Pure]
    public static System.DateTimeOffset AddBusinessDays(this System.DateTimeOffset dateTimeOffset, int businessDays, TimeZoneInfo? zone = null)
    {
        if (businessDays == 0)
            return dateTimeOffset;

        int direction = businessDays > 0 ? 1 : -1;
        int remaining = Math.Abs(businessDays);
        System.DateTimeOffset current = dateTimeOffset;

        while (remaining > 0)
        {
            current = current.AddDays(direction);

            if (current.IsBusinessDay(zone))
                remaining--;
        }

        return current;
    }

    /// <summary>
    /// Checks whether the value is between <paramref name="start"/> and <paramref name="end"/>.
    /// </summary>
    [Pure]
    public static bool IsBetween(this System.DateTimeOffset value, System.DateTimeOffset start, System.DateTimeOffset end, bool inclusive = true)
    {
        if (start > end) (start, end) = (end, start);

        return inclusive ? value >= start && value <= end : value > start && value < end;
    }
}
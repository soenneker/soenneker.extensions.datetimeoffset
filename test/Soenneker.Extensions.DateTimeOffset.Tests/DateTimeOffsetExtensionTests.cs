using AwesomeAssertions;
using Soenneker.Tests.Unit;
using System;
using System.Runtime.InteropServices;
using Xunit;

namespace Soenneker.Extensions.DateTimeOffset.Tests;

public class DateTimeOffsetExtensionTests : UnitTest
{
    [Fact]
    public void ToUtcDateTime_ConvertsToUtcCorrectly()
    {
        // Arrange
        // Create a DateTimeOffset for 3 PM UTC-5 (Eastern Standard Time without considering daylight saving)
        var originalDateTimeOffset = new System.DateTimeOffset(2023, 10, 15, 15, 0, 0, new TimeSpan(-5, 0, 0));

        // Expected DateTime in UTC is 8 PM on the same day
        var expectedDateTime = new DateTime(2023, 10, 15, 20, 0, 0, DateTimeKind.Utc);

        // Act
        DateTime result = originalDateTimeOffset.ToUtcDateTime();

        // Assert
        result.Should().Be(expectedDateTime);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    private static TimeZoneInfo GetCentralTimeZone()
    {
        // Cross-platform IANA / Windows mapping
        string id = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Central Standard Time" : "America/Chicago";
        return TimeZoneInfo.FindSystemTimeZoneById(id);
    }

    /* ─────────────────────────────  ToUtcDateTime  ───────────────────────────── */

    [Fact]
    public void ToUtcDateTime_returns_same_value_as_property()
    {
        var dto = new System.DateTimeOffset(2025, 07, 09, 08, 15, 00, TimeSpan.FromHours(-5)); // 08:15 CDT
        dto.ToUtcDateTime().Should().Be(dto.UtcDateTime);
    }

    /* ─────────────────────────────  IsBusinessDay  ───────────────────────────── */

    [Fact]
    public void IsBusinessDay_without_zone_uses_instance_offset()
    {
        var friday = new System.DateTimeOffset(2025, 07, 04, 09, 00, 00, TimeSpan.FromHours(-5)); // Friday
        var saturday = new System.DateTimeOffset(2025, 07, 05, 09, 00, 00, TimeSpan.FromHours(-5)); // Saturday

        friday.IsBusinessDay().Should().BeTrue();
        saturday.IsBusinessDay().Should().BeFalse();
    }

    [Fact]
    public void IsBusinessDay_with_zone_uses_specified_timezone()
    {
        // 23:00 CDT Friday == 04:00 UTC Saturday
        var fridayLateCdt = new System.DateTimeOffset(2025, 07, 04, 23, 00, 00, TimeSpan.FromHours(-5));

        fridayLateCdt.IsBusinessDay().Should().BeTrue(); // Friday in CDT
        fridayLateCdt.IsBusinessDay(TimeZoneInfo.Utc).Should().BeFalse(); // Saturday in UTC
    }

    /* ─────────────────────────────  AddBusinessDays  ───────────────────────────── */

    [Fact]
    public void AddBusinessDays_skips_weekends_forward()
    {
        var friday = new System.DateTimeOffset(2025, 07, 04, 12, 00, 00, TimeSpan.Zero); // Friday
        var result = friday.AddBusinessDays(1);

        result.Should().Be(friday.AddDays(3)); // Monday
    }

    [Fact]
    public void AddBusinessDays_skips_weekends_backward()
    {
        var monday = new System.DateTimeOffset(2025, 07, 07, 12, 00, 00, TimeSpan.Zero); // Monday
        var result = monday.AddBusinessDays(-1);

        result.Should().Be(monday.AddDays(-3)); // Previous Friday
    }

    [Fact]
    public void AddBusinessDays_respects_timezone()
    {
        // 23:00 UTC Friday => 18:00 CDT Friday
        var fridayUtc = new System.DateTimeOffset(2025, 07, 04, 23, 00, 00, TimeSpan.Zero);
        var zone = GetCentralTimeZone();

        var mondayUtc = fridayUtc.AddBusinessDays(1, zone);

        mondayUtc.UtcDateTime.DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void AddBusinessDays_zero_returns_same_value()
    {
        var now = System.DateTimeOffset.UtcNow;
        now.AddBusinessDays(0).Should().Be(now);
    }

    /* ─────────────────────────────  IsBetween  ───────────────────────────── */

    [Fact]
    public void IsBetween_inclusive_true_on_boundaries()
    {
        var start = new System.DateTimeOffset(2025, 07, 09, 10, 00, 00, TimeSpan.Zero);
        var end = new System.DateTimeOffset(2025, 07, 09, 12, 00, 00, TimeSpan.Zero);

        start.IsBetween(start, end).Should().BeTrue(); // lower boundary
        end.IsBetween(start, end).Should().BeTrue(); // upper boundary
    }

    [Fact]
    public void IsBetween_exclusive_false_on_boundaries()
    {
        var start = new System.DateTimeOffset(2025, 07, 09, 10, 00, 00, TimeSpan.Zero);
        var end = new System.DateTimeOffset(2025, 07, 09, 12, 00, 00, TimeSpan.Zero);

        start.IsBetween(start, end, inclusive: false).Should().BeFalse();
        end.IsBetween(start, end, inclusive: false).Should().BeFalse();
    }

    [Fact]
    public void IsBetween_true_for_values_inside_range()
    {
        var start = new System.DateTimeOffset(2025, 07, 09, 10, 00, 00, TimeSpan.Zero);
        var end = new System.DateTimeOffset(2025, 07, 09, 12, 00, 00, TimeSpan.Zero);
        var mid = new System.DateTimeOffset(2025, 07, 09, 11, 00, 00, TimeSpan.Zero);

        mid.IsBetween(start, end).Should().BeTrue();
    }

    [Fact]
    public void IsBetween_handles_unsorted_boundaries()
    {
        var earlier = new System.DateTimeOffset(2025, 07, 01, 00, 00, 00, TimeSpan.Zero);
        var later = new System.DateTimeOffset(2025, 07, 31, 23, 59, 59, TimeSpan.Zero);
        var target = new System.DateTimeOffset(2025, 07, 15, 12, 00, 00, TimeSpan.Zero);

        target.IsBetween(later, earlier).Should().BeTrue(); // arguments intentionally reversed
    }
}
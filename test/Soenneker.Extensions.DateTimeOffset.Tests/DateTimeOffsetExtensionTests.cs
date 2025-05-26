using AwesomeAssertions;
using Soenneker.Tests.Unit;
using System;
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
}

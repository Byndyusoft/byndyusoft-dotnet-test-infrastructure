namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples;

using System;
using FluentAssertions;
using global::Byndyusoft.DotNet.Testing.Infrastructure.Extensions;
using TestCases;
using Xunit;

/// <summary>
///     Примеры тестов для <see cref="EquivalencyAssertionOptionsExtensions.CheckAllDateTimesBeClosedTo"/>
/// </summary>
public class CheckAllDateTimesBeClosedToExamplesTests
{
    [Fact]
    public void CheckAllDateTimesBeClosedTo_AllDateTimeCloseToEachOtherWith5seconds_ShouldPass()
    {
        // Arrange
        var actual = new TestEntity
                         {
                             CreateDate = DateTime.UtcNow,
                             UpdateDate = DateTime.UtcNow
                         };
        var expected = new TestEntity
                           {
                               CreateDate = DateTime.UtcNow.AddSeconds(4),
                               UpdateDate = DateTime.UtcNow.AddSeconds(-5)
                           };

        // Act
        actual.Should().BeEquivalentTo(expected, options => options.CheckAllDateTimesBeClosedTo(TimeSpan.FromSeconds(5)));

        // Assert
    }

    [Fact(Skip = "Этот тест должен падать")]
    public void CheckAllDateTimesBeClosedTo__AllDateTimeNotCloseToEachOtherWith5seconds_ShouldFail()
    {
        // Arrange
        var actual = new TestEntity
                         {
                             CreateDate = DateTime.UtcNow,
                             UpdateDate = DateTime.UtcNow
                         };
        var expected = new TestEntity
                           {
                               CreateDate = DateTime.UtcNow.AddSeconds(10),
                               UpdateDate = DateTime.UtcNow.AddSeconds(-10)
                           };


        // Act
        actual.Should().BeEquivalentTo(expected, options => options.CheckAllDateTimesBeClosedTo(TimeSpan.FromSeconds(5)));

        // Assert
    }
}
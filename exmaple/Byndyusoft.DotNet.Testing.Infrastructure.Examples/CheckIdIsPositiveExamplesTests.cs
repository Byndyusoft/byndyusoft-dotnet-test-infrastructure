namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples;

using FluentAssertions;
using global::Byndyusoft.DotNet.Testing.Infrastructure.Extensions;
using TestCases;
using Xunit;

/// <summary>
///     Примеры тестов с использованием <see cref="EquivalencyAssertionOptionsExtensions.CheckIdIsPositive"/>
/// </summary>
public class CheckIdIsPositiveExamplesTests
{
    //[Fact]
    public void CheckIdIsPositive_IdPositive_ShouldPass()
    {
        // Arrange
        var actual = new TestEntity { Id = 1 };
        var expected = new TestEntity { Id = 1 };

        // Act
        actual.Should().BeEquivalentTo(expected, options => options.CheckIdIsPositive());

        // Assert
    }

    //[Fact(Skip = "Этот тест должен падать")]
    public void CheckIdIsPositive_IdNegative_ShouldFail()
    {
        // Arrange
        var actual = new TestEntity { Id = -1 };
        var expected = new TestEntity { Id = -1 };

        // Act
        actual.Should().BeEquivalentTo(expected, options => options.CheckIdIsPositive());

        // Assert
    }
}
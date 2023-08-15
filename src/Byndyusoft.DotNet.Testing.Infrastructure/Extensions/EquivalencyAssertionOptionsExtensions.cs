namespace Byndyusoft.DotNet.Testing.Infrastructure.Extensions;

using System;
using FluentAssertions;
using FluentAssertions.Equivalency;

/// <summary>
///     Методы-расширения для EquivalencyAssertionOptions из FluentAssertions.Equivalency
/// </summary>
public static class EquivalencyAssertionOptionsExtensions
{
    /// <summary>
    ///     Проверяет, что в проверяемой сущности поле с названием "Id" является положительным числом типа int
    /// </summary>
    public static EquivalencyAssertionOptions<TType> CheckIdIsPositive<TType>(
        this EquivalencyAssertionOptions<TType> options)
    {
        return options.Using<int>(context => context.Subject.Should().BePositive())
                      .When(info => info.SelectedMemberInfo != null && info.SelectedMemberInfo.Name == "Id");
    }

    /// <summary>
    ///     Проверяет, что в проверяемой сущности все значения типа DateTime находятся в пределах определённого интервала от
    ///     заданного момента времени
    /// </summary>
    public static EquivalencyAssertionOptions<TType> CheckAllDateTimesBeClosedTo<TType>(
        this EquivalencyAssertionOptions<TType> options, TimeSpan precision)
    {
        return options
               .Using<DateTime>(context => context.Subject.Should().BeCloseTo(context.Expectation, precision))
               .WhenTypeIs<DateTime>();
    }
}
namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples.TestCases;

using System;

/// <summary>
///     Тестовая сущность
/// </summary>
public class TestEntity
{
    /// <summary>
    ///     Идентификатор
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Дата создания
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    ///     Дата изменения
    /// </summary>
    public DateTime UpdateDate { get; set; }
}
namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities;

using System;
using System.Linq;

/// <summary>
///     Тест кейс
/// </summary>
public class TestCase
{
    /// <summary>
    ///     Идентификатор тест кейса
    /// </summary>
    public string? TestId { get; set; }

    /// <summary>
    ///     Имя тест кейса
    /// </summary>
    public string? Name => (Description ?? string.Empty)
                           .Split(Environment.NewLine)
                           .FirstOrDefault(x => string.IsNullOrWhiteSpace(x) == false);

    /// <summary>
    ///     Описание тест кейса
    /// </summary>
    public string? Description { get; set; } = default!;

    /// <summary>
    ///     Категория - для разделения кейсов на первом уровне
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    ///     Подкатегория - для разделения кейсов на втором уровне
    /// </summary>
    public string? SubCategory { get; set; }

    /// <summary>
    ///     Тип, описывающий тест-кейс
    /// </summary>
    public Type TestCaseItemType { get; set; } = default!;

    /// <summary>
    ///     Тип, продуцирующий экземпляр тест-кейса типа <see cref="TestCaseItemType"/> 
    /// </summary>
    public Type TestCaseType { get; set; } = default!;

    /// <summary>
    ///     Тестовые методы, где используется тест кейс
    /// </summary>
    public string[] TestMethods { get; set; } = default!;

    /// <summary>
    ///     True, если тест кейс был задублирован по категории, подкатегории и идентификатору
    /// </summary>
    public bool Duplicated { get; set; }

    /// <summary>
    ///     True, если тест кейс автоматизирован
    /// </summary>
    public bool IsAutomated => TestMethods.Any();
}
namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities;

/// <summary>
///     Категория тест кейсов
/// </summary>
internal sealed class ReadmeSubCategory
{
    /// <summary>
    ///     Имя подкатегории
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Описание подкатегории
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Порядковый номер подкатегории в категории
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    ///     Количество тест-кейсов в подкатегории
    /// </summary>
    public int TestsCount { get; set; }

    /// <summary>
    ///     Тест-кейсы
    /// </summary>
    public TestCase[] TestCases { get; set; } = default!;
}
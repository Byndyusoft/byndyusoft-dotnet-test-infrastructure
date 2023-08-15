namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities;

/// <summary>
///     Настройки построения markdown отчёта по тест кейсам
/// </summary>
public class TestCaseMarkdownReportingOptions
{
    /// <summary>
    ///     Путь к файлу с шаблоном отчёта
    /// </summary>
    public string? TemplatePath { get; set; }
}
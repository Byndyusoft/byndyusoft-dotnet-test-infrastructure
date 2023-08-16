namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities;

/// <summary>
///     Настройки создания отчёта по тест кейсам
/// </summary>
public class TestCaseReportingOptions
{
    /// <summary>
    ///     Имя итогового файла с отчётом
    /// </summary>
    public string? ReportName { get; set; }

    /// <summary>
    ///     Путь к файлу с шаблоном отчёта
    /// </summary>
    public string? TemplatePath { get; set; }
}
namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities;

/// <summary>
///     Консистетность отчёта
/// </summary>
public enum ReportConsistency
{
    /// <summary>
    ///     Отчёт консистентен
    /// </summary>
    Consistent = 1,

    /// <summary>
    ///     Отчёт содержит описание ошибок в тест-кейсах и\или шаблона
    /// </summary>
    Inconsistent = 2
}
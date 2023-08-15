namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities;

using System.Collections.Generic;
using System.Linq;

/// <summary>
///     Ошибки формирования отчёта
/// </summary>
internal sealed class ReportErrors
{
    /// <summary>
    ///     Ошибки формирования тест кейсов
    /// </summary>
    public string? TestCaseErrors { get; set; }

    /// <summary>
    ///     Ошибки валидации шаблона
    /// </summary>
    public string? TemplateValidationErrors { get; set; }

    /// <summary>
    ///     Тест кейсы с пустыми идентификаторами
    /// </summary>
    public TestCase[] EmptyIds { get; set; } = default!;

    /// <summary>
    ///     Тест кейсы с пустыми описаниями
    /// </summary>
    public TestCase[] EmptyDescriptions { get; set; } = default!;

    /// <summary>
    ///     Тест кейсы с дублированыными идентификаторами
    /// </summary>
    public Dictionary<string, TestCase[]> DuplicateTestIds { get; set; } = default!;

    /// <summary>
    ///     Возвращает true, если есть ошибки формирования тест кейсов
    /// </summary>
    public bool HasTestCaseErrors => string.IsNullOrWhiteSpace(TestCaseErrors) == false;

    /// <summary>
    ///     Возвращает true, если есть ошибки ошибки валидации шаблона
    /// </summary>
    public bool HasTemplateValidationErrors => string.IsNullOrWhiteSpace(TemplateValidationErrors) == false;

    /// <summary>
    ///     Возвращает true, если есть пустые идентификаторы тест-кейсов 
    /// </summary>
    public bool HasEmptyIds => EmptyIds.Any();

    /// <summary>
    ///     Возвращает true, если есть пустые описания тест кейсов 
    /// </summary>
    public bool HasEmptyDescriptions => EmptyDescriptions.Any();

    /// <summary>
    ///     Возвращает true, если есть дублирванные идентификаторы тест-кейсов 
    /// </summary>
    public bool HasDuplicateTestIds => DuplicateTestIds.Any();
        
    /// <summary>
    ///     Возвращает true, если есть ошибки 
    /// </summary>
    public bool HasErrors => HasTestCaseErrors ||
                             HasTemplateValidationErrors ||
                             HasEmptyIds ||
                             HasEmptyDescriptions ||
                             HasDuplicateTestIds;
}
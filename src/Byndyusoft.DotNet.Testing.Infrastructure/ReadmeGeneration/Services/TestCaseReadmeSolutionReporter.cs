namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Services;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Entities;
using Interfaces;

/// <summary>
///     Служба создания readme документа по тест кейсам решения
/// </summary>
public sealed class TestCaseReadmeSolutionReporter : ITestCaseSolutionReporter
{
    private readonly ITestCaseReportBuilder _testCaseReportBuilder;

    /// <summary>
    ///     Имя файла для генерации документа по умолчанию
    /// </summary>
    private const string ReadmeName = "README_TestCases.md";

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="testCaseReportBuilder">Служба формирования отчёта</param>
    private TestCaseReadmeSolutionReporter(ITestCaseReportBuilder testCaseReportBuilder)
    {
        _testCaseReportBuilder = testCaseReportBuilder;
    }

    /// <summary>
    ///     Возвращает новый экземпляр службы
    /// </summary>
    /// <param name="options">Настройки генерации markdown</param>
    public static TestCaseReadmeSolutionReporter New(TestCaseMarkdownReportingOptions options)
    {
        return new TestCaseReadmeSolutionReporter(new TestCaseReadmeReportBuilder(new TestCaseReportModelBuilder(new TestCasesExtractor(), options)));
    }

    /// <summary>
    ///     Возвращает новый экземпляр службы
    /// </summary>
    public static TestCaseReadmeSolutionReporter New()
    {
        return New(new TestCaseMarkdownReportingOptions());
    }

    /// <summary>
    ///     Генерирует документ readme по тест кейсам решения
    /// </summary>
    /// <returns>
    ///     Возвращает true, если в отчёте есть ошибки
    /// </returns>
    public async Task<bool> Generate(params Assembly[] assemblies)
    {
        var (report, hasErrors) = await _testCaseReportBuilder.Build(assemblies);
        var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
        var solutionDir = GetSolutionDirectory(currentDir);
        if (solutionDir is null)
            throw new Exception("Корневой каталог решения (solution'а) не найден");

        var reportPath = Path.Combine(solutionDir.FullName, ReadmeName);
        await File.WriteAllTextAsync(reportPath, report);
            
        return hasErrors;
    }

    /// <summary>
    ///     Возвращает корневой каталог решения (solution'а) 
    /// </summary>
    /// <param name="dir">Подкаталог решения</param>
    /// <remarks>
    ///     Рекурсивно ищет корневой каталог решения среди переданного каталога и его родительских каталогов
    /// </remarks>
    /// <returns>
    ///     Возвращает null, если найти корневой каталог решения не удалось
    /// </returns>
    private DirectoryInfo? GetSolutionDirectory(DirectoryInfo? dir)
    {
        if (dir == null)
            return null;

        if (dir.GetFiles().Any(f => string.Equals(f.Extension.TrimStart('.'), "sln", StringComparison.OrdinalIgnoreCase)))
            return dir;
            
        return GetSolutionDirectory(dir.Parent);
    }
}
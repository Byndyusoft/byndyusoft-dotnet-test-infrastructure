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
    /// <summary>
    ///     Расширение файла решения 
    /// </summary>
    const string SolutionExtenstion = "sln";

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
    public async Task<bool> BuildAndSave(params Assembly[] assemblies)
    {
        var (report, hasErrors) = await _testCaseReportBuilder.Build(assemblies);
        var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
        var solutionDir = GetSolutionDirectory(currentDir);
        if (solutionDir is null)
            throw new Exception("Корневой каталог решения (solution'а) не найден");

        var reportPath = Path.Combine(solutionDir.FullName, ReadmeName);
#if NETSTANDARD
        File.WriteAllText(reportPath, report);
#else
        await File.WriteAllTextAsync(reportPath, report);
#endif
            
        return hasErrors;
    }

    /// <summary>
    ///     Возвращает корневой каталог решения (solution'а) 
    /// </summary>
    /// <param name="solutionSubDir">Подкаталог решения</param>
    /// <remarks>
    ///     Рекурсивно ищет корневой каталог решения среди переданного каталога и его родительских каталогов
    /// </remarks>
    /// <returns>
    ///     Возвращает null, если найти корневой каталог решения не удалось
    /// </returns>
    private DirectoryInfo? GetSolutionDirectory(DirectoryInfo? solutionSubDir)
    {
        if (solutionSubDir is null)
            return null;

        if (solutionSubDir.GetFiles().Any(f => string.Equals(f.Extension.TrimStart('.'), SolutionExtenstion, StringComparison.OrdinalIgnoreCase)))
            return solutionSubDir;
            
        return GetSolutionDirectory(solutionSubDir.Parent);
    }
}
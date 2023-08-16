namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Services;

using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Entities;
using Interfaces;

/// <summary>
///     Билдер объектной модели отчёта
/// </summary>
internal sealed class TestCaseReportModelBuilder : ITestCaseReportModelBuilder
{
    private readonly ITestCaseExtractor _testCaseExtractor;
    private readonly TestCaseReportingOptions _reportingOptions;

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="testCaseExtractor">Служба извлечения тест кейсов</param>
    /// <param name="reportingOptions">Настройки построения отчёта</param>
    public TestCaseReportModelBuilder(ITestCaseExtractor testCaseExtractor, TestCaseReportingOptions reportingOptions)
    {
        _testCaseExtractor = testCaseExtractor;
        _reportingOptions = reportingOptions;
    }

    /// <summary>
    ///     Возвращает объектную модель отчёта по тест кейсам
    /// </summary>
    /// <param name="assemblies">Сборки для поиска искать тест кейсы</param>
    public async Task<ReadmeReport> Build(params Assembly[] assemblies)
    {
        // получаем шаблон
        var (template, templateErrors) = await GetTemplate();

        // извлекаем тест-кейсы
        var (testCases, testCaseErrors) = _testCaseExtractor.Get(assemblies);

        // формируем отчёт
        var readmeReport = GetReport(testCases, template, templateErrors, testCaseErrors);

        // возвращаем результат
        return readmeReport;
    }

    /// <summary>
    ///     Возвращает объектную модель отчёта по тесткейсам
    /// </summary>
    /// <param name="testCases">Тест кейсы</param>
    /// <param name="template">Шаблон отчёта</param>
    /// <param name="templateErrors">Ошибки разбора шаблона</param>
    /// <param name="testCaseErrors">Ошибки формирования тест кейсов</param>
    private ReadmeReport GetReport(TestCase[] testCases, TemplateNode template, string? templateErrors, string? testCaseErrors)
    {
        // получаем категории
        var categories = testCases.GroupBy(testCase => testCase.Category)
                                  .Select(category => CreateCategory(category, template))
                                  .ToArray();

        // отчёт
        var report = new ReadmeReport(template.Name, template.Description, categories, templateErrors, testCaseErrors);

        return report;
    }

    /// <summary>
    ///     Возвращает категорию тест кейсов
    /// </summary>
    /// <param name="category">Группа тест кейсов категории</param>
    /// <param name="template">Шаблон</param>
    private ReadmeCategory CreateCategory(IGrouping<string?, TestCase> category, TemplateNode template)
    {
        return new ReadmeCategory
        {
            Name = category.Key,
            Description = template.GetCategoryDescription(category.Key),
            TestsCount = category.Count(),
            Order = template.GetCategoryOrder(category.Key),
            SubCategories = category.GroupBy(c => c.SubCategory)
                                    .Select(subCategory => CreateSubCategory(subCategory, category.Key, template))
                                    .ToArray()
        };
    }

    /// <summary>
    ///     Возвращает подкатегорию тест кейсов
    /// </summary>
    /// <param name="subCategory">Группа тест кейсов подкатегории</param>
    /// <param name="categoryName">Имя родительской категории</param>
    /// <param name="template">Шаблон</param>
    private ReadmeSubCategory CreateSubCategory(IGrouping<string?, TestCase> subCategory,
                                                string? categoryName,
                                                TemplateNode template)
    {
        return new ReadmeSubCategory
        {
            Name = subCategory.Key,
            Description = template.GetSubCategoryDescription(categoryName, subCategory.Key),
            TestsCount = subCategory.Count(),
            Order = template.GetSubCategoryOrder(categoryName, subCategory.Key),
            TestCases = subCategory.ToArray()
        };
    }


    /// <summary>
    ///     Возвращает шаблон отчёта
    /// </summary>
    /// <returns>
    ///     Вернёт ошибку валидации шаблона, если шаблон некорректный
    /// </returns>
    private async Task<(TemplateNode Suite, string? ValidationErrors)> GetTemplate()
    {
        // шаблон по умолчанию
        var defaultTemplate = TemplateNode.CreateRoot("Тест-кейсы");

        // проверям наличие настройки
        if (_reportingOptions.TemplatePath is null)
            return (defaultTemplate, null);

        // проверяем наличие шаблона
        if (File.Exists(_reportingOptions.TemplatePath) == false)
            return (defaultTemplate, $"Не найден шаблон readme по пути {_reportingOptions.TemplatePath}");

        // создаём корень 
        var currentNode = TemplateNode.CreateRoot();
        using (var sr = new StreamReader(File.OpenRead(_reportingOptions.TemplatePath)))
        {
            // счётчик строк
            int linesCount = 0;

            // читаем шаблон построчно
            while (await sr.ReadLineAsync() is { } line)
            {
                linesCount++;

                // добавляем ноду в шаблон
                var (nextNode, error) = currentNode.AddNode(line);
                if (string.IsNullOrEmpty(error) == false)
                    return (defaultTemplate, $"{error}. Строка {linesCount}");

                // подменяем текущую ноду
                currentNode = nextNode!;
            }
        }

        // получаем корень шаблона
        var result = currentNode.Root;

        return (result, null);
    }
}
namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Services
{
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
        private readonly TestCaseMarkdownReportingOptions _reportingOptions;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="testCaseExtractor">Служба извлечения тест кейсов</param>
        /// <param name="reportingOptions">Настройки построения отчёта</param>
        public TestCaseReportModelBuilder(ITestCaseExtractor testCaseExtractor, TestCaseMarkdownReportingOptions reportingOptions)
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
            var (template, errors) = await GetTemplate();

            // извлекаем тест-кейсы
            var testCases = _testCaseExtractor.Get(assemblies);

            // формируем отчёт
            var readmeReport = GetReport(testCases, template, errors);

            // возвращаем результат
            return readmeReport;
        }

        /// <summary>
        ///     Возвращает объектную модель отчёта по тесткейсам
        /// </summary>
        /// <param name="testCases">Тест кейсы</param>
        /// <param name="template">Шаблон отчёта</param>
        /// <param name="errors">Ошибки разбора шаблона</param>
        private ReadmeReport GetReport(TestCase[] testCases, TemplateNode template, string? errors)
        {
            // отчёт
            var report = new ReadmeReport
                             {
                                 Name = template.Name,
                                 Description = template.Description,
                                 Categories = testCases.GroupBy(testCase => testCase.Category)
                                                       .Select(category => new ReadmeCategory
                                                                               {
                                                                                   Name = category.Key,
                                                                                   Description = template.GetCategoryDescription(category.Key),
                                                                                   TestCount = category.Count(),
                                                                                   Order = template.GetCategoryOrder(category.Key),
                                                                                   SubCategories = category.GroupBy(c => c.SubCategory)
                                                                                                           .Select(subCategory => new ReadmeSubCategory
                                                                                                                                      {
                                                                                                                                          Name = subCategory.Key,
                                                                                                                                          Description = template.GetSubCategoryDescription(category.Key, subCategory.Key),
                                                                                                                                          TestCount = subCategory.Count(),
                                                                                                                                          Order = template.GetSubCategoryOrder(category.Key, subCategory.Key),
                                                                                                                                          TestCases = subCategory.ToArray()
                                                                                                                                      })
                                                                                                           .ToArray()
                                                                               })
                                                       .ToArray(),
                                 TemplateValidationErrors = errors
                             };

            return report;
        }

        /// <summary>
        ///     Возвращает шаблон отчёта
        /// </summary>
        /// <returns>
        ///     Вернёт ошибку валидации шаблона, если шаблон некорректный
        /// </returns>
        private async Task<(TemplateNode Suite, string? ValidationErrors)> GetTemplate()
        {
            // проверям наличие настройки
            if (_reportingOptions.TemplatePath is null)
                return (TemplateNode.Default(), null);

            // проверяем наличие шаблона
            if (File.Exists(_reportingOptions.TemplatePath) == false)
                return (TemplateNode.Default(), $"Не найден шаблон readme по пути {_reportingOptions.TemplatePath}");

            // создаём корень 
            var currentNode = TemplateNode.CreateRoot();
            using (var sr = new StreamReader(File.OpenRead(_reportingOptions.TemplatePath)))
            {
                // счётчик строк
                int count = 0;

                // читаем шаблон построчно
                while (await sr.ReadLineAsync() is { } line)
                {
                    count++;

                    // добавляем ноду в шаблон
                    var (nextNode, error) = currentNode.AddNode(line);
                    if (string.IsNullOrEmpty(error) == false)
                        return (TemplateNode.Default(), $"{error}. Строка {count}");

                    // подменяем текущую ноду
                    currentNode = nextNode!;
                }
            }

            // получаем корень шаблона
            var result = currentNode.Root;

            return (result, null);
        }
    }
}
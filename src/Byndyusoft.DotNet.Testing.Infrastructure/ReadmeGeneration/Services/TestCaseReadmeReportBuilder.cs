namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Services
{
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Interfaces;

    /// <summary>
    ///     Служба формирования отчёта по тест кейсам в формате README.MD репозитория
    /// </summary>
    internal sealed class TestCaseReadmeReportBuilder : ITestCaseReportBuilder
    {
        private readonly ITestCaseReportModelBuilder _testCaseReportModelBuilder;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="testCaseReportModelBuilder">Служба построения объектной модели отчёта</param>
        public TestCaseReadmeReportBuilder(ITestCaseReportModelBuilder testCaseReportModelBuilder)
        {
            _testCaseReportModelBuilder = testCaseReportModelBuilder;
        }

        /// <summary>
        ///     Возвращает строковое представление отчёта по тест кейсам из переданных сборок
        /// </summary>
        /// <returns>
        ///     Также возвращает флаг наличия ошибок в отчёте
        /// </returns>
        public async Task<(string, bool)> Build(params Assembly[] assemblies)
        {
            // формируем отчёт
            var readmeReport = await _testCaseReportModelBuilder.Build(assemblies);

            // билдер разметки
            var markupBuilder = new ReadmeMarkupBuilder(readmeReport);

            // идём по категориям отчёта
            foreach (var category in readmeReport.Categories
                                                 .OrderBy(c => c.Order)
                                                 .ThenBy(c => c.Name))
            {
                // добавляем разметку категории в отчёт
                markupBuilder.AddCategory(category);

                // идём по подкатегориям категории
                foreach (var subCategory in category.SubCategories
                                                    .OrderBy(s => s.Order)
                                                    .ThenBy(c => c.Name))
                {
                    // добавляем разметку подкатегории в отчёт
                    markupBuilder.AddSubCategory(category, subCategory);

                    // идём по тесткейсам категории
                    foreach (var testCase in subCategory.TestCases
                                                        .OrderBy(t => t.Name)
                                                        .ThenBy(t => t.TestCaseType.FullName))
                    {
                        // добавляем разметку тесткейса в отчёт
                        markupBuilder.AddTestCase(testCase);
                    }
                }
            }

            // возвращаем результат
            return (markupBuilder.Build(), readmeReport.Validate().HasErrors);
        }
    }
}
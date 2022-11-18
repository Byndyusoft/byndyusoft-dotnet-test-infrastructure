namespace Byndyusoft.DotNet.Testing.Infrastructure.Services
{
    using System.Reflection;
    using System.Text;

    /// <summary>
    ///     Служба для формирования отчета по тест-кейсам в формате CSV
    /// </summary>
    public sealed class TestCaseCsvReporter : ITestCaseReporter
    {
        /// <summary>
        ///     Служба извлечения тест-кейсов
        /// </summary>
        private readonly ITestCasesExtractor _testCasesExtractor;

        /// <summary>
        ///     Сепаратор для CSV
        /// </summary>
        private readonly string _separator;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="separator">Сепаратор для CSV</param>
        public TestCaseCsvReporter(string separator = ";")
            : this(new TestCasesExtractor(), separator)
        {

        }

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="testCasesExtractor">Служба извлечения тест-кейсов</param>
        /// <param name="separator">Сепаратор для CSV</param>
        public TestCaseCsvReporter(ITestCasesExtractor testCasesExtractor, string separator = ";")
        {
            _testCasesExtractor = testCasesExtractor;
            _separator = separator;
        }

        /// <summary>
        ///     Возвращает строкове представление CSV отчёта по тесткейсам из переданных сборок
        /// </summary>
        public string Build(params Assembly[] assemblies)
        {
            var testCases = _testCasesExtractor.Get(assemblies);
            var csvBuilder = new StringBuilder();

            foreach (var testCase in testCases)
            {
                csvBuilder.Append(testCase.TestId)
                          .Append(_separator)
                          .Append(testCase.Duplicated)
                          .AppendLine();
            }

            return csvBuilder.ToString();
        }
    }
}
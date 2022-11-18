namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples
{
    using System.Reflection;
    using FluentAssertions;
    using global::Byndyusoft.DotNet.Testing.Infrastructure.Services;
    using TestCases;
    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    ///     Примеры тестов для сбора тест-кейсов
    /// </summary>
    public class TestCasesReporterRunTests
    {
        private readonly ITestOutputHelper _output;

        public TestCasesReporterRunTests(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        ///     Пример использования конвенциональных тест кейсов
        /// </summary>
        /// <param name="testCase"></param>
        [ClassData(typeof(ExampleTestCases))]
        [Theory]
        public void Actual_And_Expected_ShouldBeEquivalent(TestCaseItem testCase)
        {
            // Arrange
            _output.WriteLine(testCase.ToStringDescription());

            // Act
            var actual = testCase.Expected; // здесь тестируемый вызов

            // Assert
            actual.Should().BeEquivalentTo(testCase.Expected);
        }

        /// <summary>
        ///     Пример сбора тест-кейсов из текущей исполняемой сборки
        /// </summary>
        [Fact]
        public void GetTestCases_FromExecutingAssembly_WriteThemToConsole()
        {
            // Arrange
            var testsAssembly = Assembly.GetExecutingAssembly();

            // Act
            var testCases = new TestCasesExtractor().Get(testsAssembly);

            // Assert
            foreach (var testCase in testCases)
            {
                var duplicatedLabel = testCase.Duplicated ? "- Duplicated" : string.Empty;
                _output.WriteLine($"{testCase.TestId}{duplicatedLabel}");
            }
        }

        /// <summary>
        ///     Пример формирование CSV отчёта по тест-кейсам из текущей исполняемой сборки
        /// </summary>
        [Fact]
        public void GetTestCases_FromExecutingAssembly_WriteCsvReportToConsole()
        {
            // Arrange
            var testsAssembly = Assembly.GetExecutingAssembly();

            // Act
            var csv = new TestCaseCsvReporter().Build(testsAssembly);

            // Assert
            _output.WriteLine(csv);
        }
    }
}

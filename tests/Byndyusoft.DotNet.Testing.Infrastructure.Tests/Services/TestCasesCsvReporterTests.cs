namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Tests.Services
{
    using System.Reflection;
    using FluentAssertions;
    using global::Byndyusoft.DotNet.Testing.Infrastructure.Services;
    using TestCases;
    using Xunit;

    public class TestCasesExtractorTests
    {
        [Fact]
        public void Get_FromExecutingAssembly_ReturnsConventionalTestCases()
        {
            // Arrange
            var testsAssembly = Assembly.GetExecutingAssembly();

            var expected = new TestCase[]
            {
                new TestCase
                {
                    TestId = "TestId_01",
                    Description = "Description for test case 01",
                    ProducerType = typeof(InstanceTestCaseItem),
                    TestCaseType = typeof(TestCaseItem),
                    Duplicated = false
                },
                new TestCase
                {
                    TestId = "TestId_02",
                    Description = "Description for test case 02",
                    ProducerType = typeof(StaticTestCaseItem),
                    TestCaseType = typeof(TestCaseItem),
                    Duplicated = false
                },
                new TestCase
                {
                    TestId = "TestId_03",
                    Description = "Description for test case 03",
                    ProducerType = typeof(DuplicatedTestCaseItem),
                    TestCaseType = typeof(TestCaseItem),
                    Duplicated = true
                },
                new TestCase
                {
                    TestId = "TestId_03",
                    Description = "Description for test case 04",
                    ProducerType = typeof(DuplicatedTestCaseItem),
                    TestCaseType = typeof(TestCaseItem),
                    Duplicated = true
                }
            };

            // Act
            var testCases = new TestCasesExtractor().Get(testsAssembly);

            // Assert
            testCases.Should().BeEquivalentTo(expected, options => options.WithoutStrictOrdering());
        }
    }
}
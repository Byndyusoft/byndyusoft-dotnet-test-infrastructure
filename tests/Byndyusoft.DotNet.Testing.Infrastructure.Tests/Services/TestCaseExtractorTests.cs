namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Tests.Services
{
    using System.Reflection;
    using FluentAssertions;
    using global::Byndyusoft.DotNet.Testing.Infrastructure.Services;
    using Xunit;
    
    public class TestCaseCsvReporterTests
    {
        [Fact]
        public void Build_FromExecutingAssembly_ReturnsCsvReport()
        {
            // Arrange
            var testsAssembly = Assembly.GetExecutingAssembly();
            
            var expected = @"TestId_01;False
TestId_02;False
TestId_03;True
TestId_03;True
";

            // Act
            var csvReport = new TestCaseCsvReporter().Build(testsAssembly);

            // Assert
            csvReport.Should().Be(expected);
        }
    }
}

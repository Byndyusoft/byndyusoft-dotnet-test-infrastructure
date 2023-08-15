namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Tests.ReadmeGeneration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using global::Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities;
using global::Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Services;
using global::Byndyusoft.DotNet.Testing.Infrastructure.TestBase;
using Xunit;

/// <summary>
///     Тесты на генерацию тестовой документации в readme
/// </summary>
[Collection("tests")]
public class TestCaseReadmeSolutionReporterTests
{
    [Fact]
    public async Task Generate_FromCurrentDomain_ShouldGenerateReadmeInSolutionRoot()
    {
        // ARRANGE
        var reporter = TestCaseReadmeSolutionReporter.New();

        // ACT
        var hasErrors = await reporter.Generate(Assembly.GetExecutingAssembly());

        // ASSERT
        hasErrors.Should().BeTrue();

        var expectedReadme = new FileInfo(Path.Combine("ReadmeGeneration", "TestCases", "WithoutTemplate", "Expected.md"));
        var expectedContent = await File.ReadAllTextAsync(expectedReadme.FullName);
        expectedContent = expectedContent.Replace("\r", "");

        var solutionDir = new DirectoryInfo(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.Parent!.Parent!;
        var actualReadme = solutionDir.GetFiles("README_TestCases.md").Should().ContainSingle().Subject;
        var actualContent = await File.ReadAllTextAsync(actualReadme.FullName);
        actualContent = actualContent.Replace("\r", "");

        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public async Task Generate_FromCurrentDomain_WithTemplate_ShouldGenerateReadmeInSolutionRoot()
    {
        // ARRANGE
        var options = new TestCaseMarkdownReportingOptions
        {
            TemplatePath = Path.Combine("ReadmeGeneration", "TestCases", "WithTemplate", "README_TestCases_Template.md")
        };

        var reporter = TestCaseReadmeSolutionReporter.New(options);

        // ACT
        var hasErrors = await reporter.Generate(Assembly.GetExecutingAssembly());

        // ASSERT
        hasErrors.Should().BeTrue();

        var expectedReadme = new FileInfo(Path.Combine("ReadmeGeneration", "TestCases", "WithTemplate", "Expected.md"));
        var expectedContent = await File.ReadAllTextAsync(expectedReadme.FullName);
        expectedContent = expectedContent.Replace("\r", "");

        var solutionDir = new DirectoryInfo(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.Parent!.Parent!;
        var actualReadme = solutionDir.GetFiles("README_TestCases.md").Should().ContainSingle().Subject;
        var actualContent = await File.ReadAllTextAsync(actualReadme.FullName);
        actualContent = actualContent.Replace("\r", "");

        actualContent.Should().Be(expectedContent);
    }
}

public class TestCaseItem : TestCaseItemBase
{

}

public class SomeTestCase1 : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = "TestId1",
            Description = @"
TestName1

Description of test-case 1  
bla-bla
",
            Category = "CategorySomeFeature",
            SubCategory = "SubCategorySomeScenarios"
        };
    }
}

public class SomeTestCase2 : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = "TestId2",
            Description = @"
TestName2

Description of test-case 2  
bla-bla
",
            Category = "CategorySomeFeature",
            SubCategory = "SubCategorySomeScenarios"
        };
    }
}

public class SomeTestCase3 : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = "TestId3",
            Description = @"
TestName3

Description of test-case 3  
bla-bla
",
            Category = "CategoryAnotherFeature",
            SubCategory = "Какая-то подкатегория на русском с пробелами"
        };
    }
}

public class SomeTestCase4 : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = "TestId4",
            Description = @"
TestName4

Description of test-case 4  
bla-bla
"
        };
    }
}

public class SomeTestCase5 : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = string.Empty,
            Description = string.Empty
        };
    }
}

public class SomeTestCase6 : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = null,
            Description = null
        };
    }
}

public class SomeTestCase7 : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = "TestId1",
            Description = @"
TestName1

Description of test-case 1  
bla-bla
",
            Category = "CategorySomeFeature",
            SubCategory = "SubCategorySomeScenarios"
        };
    }
}

public class SomeTestCase8 : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = "TestId8",
            Description = @"
TestName8

Description of test-case 1  
bla-bla
",
            Category = "CategorySomeFeature",
            //SubCategory = "SubCategorySomeScenarios"
        };
    }
}

public class SomeTestCase9 : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = "TestId9",
            Description = @"
TestName9

Description of test-case 9  
bla-bla
",
            Category = "CategorySomeFeature",
            SubCategory = "SubCategorySomeScenarios"
        };
    }
}

public class FailToConstructTestCase : ITestCaseData<TestCaseItem>
{
    public TestCaseItem Get()
    {
        throw new Exception("LOL");
    }

    public TestCaseItem GetNull()
    {
        return null!;
    }

    public TestCaseItem GetActivatorCantCreateThisOne(IEnumerable<string> activatorCantCreateThisOne)
    {
        return new TestCaseItem();
    }
}

public class SomeTestCase10
{
    public TestCaseItem GetWithParamaters(int i, DateTime d, TestCaseItem tci, object o)
    {
        return new TestCaseItem
        {
            TestId = "TestId10",
            Description = @"
TestName10

Description of test-case 10  
bla-bla
",
            Category = "CategorySomeFeature",
            SubCategory = "SubCategorySomeScenarios"
        };
    }
}


[Collection("tests")]
public class SomeTests
{
    [Fact]
    public async Task Some_Test_With_Ctor()
    {
        //Arrange
        var testId1 = "TestId11";
        var testId2 = "TestId11";
        var testId3 = "TestId11";
        var testCase = new TestCaseItem
        {
            TestId = testId3,
            Description = @"
TestName11

Description of test-case 11 
bla-bla
",
            Category = "CategorySomeFeature",
            SubCategory = "SubCategorySomeScenarios"

        };

        //Act

        //Assert
    }

    [Fact]
    public async Task Some_Test_Fact()
    {
        //Arrange
        var testCase = new SomeTestCase1().Get();

        //Act

        //Assert
    }

    [Theory]
    [TestCaseData(typeof(SomeTestCase1))]
    [TestCaseData(typeof(SomeTestCase2))]
    [TestCaseData(typeof(SomeTestCase3))]
    [TestCaseData(typeof(SomeTestCase4))]
    public void Some_Test_Theory(TestCaseItem testCase)
    {
        //Arrange

        //Act

        //Assert
    }
}
namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples.TestCases
{
    using Xunit;

    public class ExampleTestCases : TheoryData<TestCaseItem>
    {
        public ExampleTestCases()
        {
            Add(InstanceTestCaseItem.Get());
            Add(StaticTestCaseItem.Get());
            Add(DuplicatedTestCaseItem.GetFirstDuplicate());
            Add(DuplicatedTestCaseItem.GetSecondDuplicate());
        }
    }
}
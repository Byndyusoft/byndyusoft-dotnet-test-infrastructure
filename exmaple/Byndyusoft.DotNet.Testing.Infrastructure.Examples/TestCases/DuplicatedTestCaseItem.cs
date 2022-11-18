namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples.TestCases
{
    using System;

    public static class DuplicatedTestCaseItem
    {
        public static TestCaseItem GetFirstDuplicate()
        {
            return new TestCaseItem
            {
                TestId = "TestId_03",
                Description = "Description for test case 03",
                Expected = new TestEntity
                {
                    Id = 2,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow
                }
            };
        }
        public static TestCaseItem GetSecondDuplicate()
        {
            return new TestCaseItem
            {
                TestId = "TestId_03",
                Description = "Description for test case 04",
                Expected = new TestEntity
                {
                    Id = 2,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow
                }
            };
        }
    }
}
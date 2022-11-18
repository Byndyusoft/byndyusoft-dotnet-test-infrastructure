namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples.TestCases
{
    using System;
    
    public static class StaticTestCaseItem
    {
        public static TestCaseItem Get()
        {
            return new TestCaseItem
            {
                TestId = "TestId_02",
                Description = "Description for test case 02",
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
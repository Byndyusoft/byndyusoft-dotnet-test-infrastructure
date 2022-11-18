namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples.TestCases
{
    using System;
    
    public class InstanceTestCaseItem
    {
        public static TestCaseItem Get()
        {
            return new TestCaseItem
            {
                TestId = "TestId_01",
                Description = "Description for test case 01",
                Expected = new TestEntity
                {
                    Id = 1,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow
                }
            };
        }
    }
}
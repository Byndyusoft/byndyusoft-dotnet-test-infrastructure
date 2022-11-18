namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Tests.Services.TestCases
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

    /// <summary>
    ///     Тест-кейсы оформленные не по конвенциям
    ///     Не будут попадать в отчёт
    /// </summary>
    public class NonConventionalTestCaseItem
    {
        /// <summary>
        ///     Экземплярный метод
        /// </summary>
        public TestCaseItem ExampleGet()
        {
            return new TestCaseItem
            {
                TestId = "TestId_05",
                Description = "Description for test case 05",
                Expected = new TestEntity
                {
                    Id = 5,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow
                }
            };
        }

        /// <summary>
        ///     Метод с параметрами
        /// </summary>
        public static TestCaseItem ParametrizedGet(string testId, string description, int id)
        {
            return new TestCaseItem
            {
                TestId = testId,
                Description = description,
                Expected = new TestEntity
                {
                    Id = id,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow
                }
            };
        }

        /// <summary>
        ///     Непубличый метод
        /// </summary>
        private static TestCaseItem PrivateGet()
        {
            return new TestCaseItem
            {
                TestId = "TestId_06",
                Description = "Description for test case 06",
                Expected = new TestEntity
                {
                    Id = 6,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow
                }
            };
        }
    }
}
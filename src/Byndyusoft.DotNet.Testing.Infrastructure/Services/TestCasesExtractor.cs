namespace Byndyusoft.DotNet.Testing.Infrastructure.Services
{
    using System.Linq;
    using System.Reflection;
    using TestBase;

    /// <summary>
    ///     Служба получения тест-кейсов из сборок с авто-тестами
    /// </summary>
    public sealed class TestCasesExtractor : ITestCasesExtractor
    {
        /// <summary>
        ///     Возвращает тест-кейсы из переданных сборок
        /// </summary>
        /// <remarks>
        ///     Соберёт тест-кейсы из всех переданных сборок
        /// </remarks>
        public TestCase[] Get(params Assembly[] assemblies)
        {
            // получаем все типы из сборок
            var types = assemblies.SelectMany(assembly => assembly.GetTypes())
                                  .ToArray();

            // получаем типы тест кейсов
            var testCaseTypes = types.Where(type => type.IsSubclassOf(typeof(TestCaseItemBase)))
                                     .ToArray();

            // получаем методы, которые возвращает экземпляры тест кейсов
            var producerMethods = types.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static))
                                       .Where(methodInfo => testCaseTypes.Contains(methodInfo.ReturnType))
                                       .ToArray();

            // создаём экземпляры всех тест кейсов
            var testCases = producerMethods.Select(CreateTestCase)
                                           .Where(testCase => testCase != null)
                                           .Select(testCase => testCase!)
                                           .OrderBy(testCase => testCase.TestId)
                                           .ToArray();

            // ищем дубликаты по TestId
            var duplicatedKeys = testCases.GroupBy(testCase => testCase.TestId)
                                          .Where(grouping => grouping.Count() > 1)
                                          .Select(grouping => grouping.Key)
                                          .ToArray();

            // проставляем флаг задублированности
            foreach (var testCase in testCases)
            {
                testCase.Duplicated = duplicatedKeys.Contains(testCase.TestId);
            }

            // возвращаем результаты
            return testCases;
        }

        /// <summary>
        ///     Создаёт экземпляр тест кейса
        /// </summary>
        /// <param name="producerMethod">Метод, производящий экземпляр</param>
        /// <returns>
        ///     Возвращает null, если создать экземпляр не удалось
        /// </returns>
        private TestCase? CreateTestCase(MethodInfo producerMethod)
        {
            try
            {
                // пробуем инстанцировать тест кейс из статического вызова без аргументов
                var instance = producerMethod.Invoke(null, parameters: null) as TestCaseItemBase;
                if (instance == null)
                    return null;

                return new TestCase
                {
                    TestId = instance.TestId,
                    Description = instance.Description,
                    TestCaseType = producerMethod.ReturnType,
                    ProducerType = producerMethod.ReflectedType
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
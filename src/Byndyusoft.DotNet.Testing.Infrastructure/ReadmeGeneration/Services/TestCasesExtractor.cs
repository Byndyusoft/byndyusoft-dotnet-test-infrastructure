namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using Entities;
    using Interfaces;
    using TestBase;
    using Xunit;

    /// <summary>
    ///     Служба получения тест-кейсов из сборок с авто-тестами
    /// </summary>
    public sealed class TestCasesExtractor : ITestCaseExtractor
    {
        /// <summary>
        ///     Возвращает тест-кейсы из переданных сборок
        /// </summary>
        public TestCase[] Get(params Assembly[] assemblies)
        {
            // получаем все типы из сборок
            var types = assemblies.SelectMany(assembly => assembly.GetTypes())
                                  .ToArray();

            // получаем типы тест кейсов
            var testCaseTypes = types.Where(type => type.IsSubclassOf(typeof(TestCaseItemBase)))
                                     .ToArray();

            // получаем методы, которые возвращает экземпляры тест кейсов
            var producerMethods = types.SelectMany(type => type.GetMethods(BindingFlags.Public |
                                                                           BindingFlags.InvokeMethod |
                                                                           BindingFlags.Instance |
                                                                           BindingFlags.Static))
                                       .Where(methodInfo => testCaseTypes.Contains(methodInfo.ReturnType))
                                       .Where(methodInfo => methodInfo.GetParameters().Any() == false)
                                       .ToArray();

            // тестовые методы
            var tests = GetTests(types, producerMethods, testCaseTypes);

            // создаём экземпляры всех тест кейсов через продьюсеры
            var declaredTestCases = producerMethods.Select(info => CreateTestCase(info, tests))
                                           .Where(testCase => testCase != null)
                                           .Select(testCase => testCase!)
                                           .OrderBy(testCase => testCase.TestId)
                                           .ToArray();

            // получаем экземпляры всех тест кейсов, созданных в тестах через new
            var inlineTestCases = tests.SelectMany(t => t.InlineTestCases)
                                       .Select(t => new TestCase
                                                        {
                                                            TestId = t.TestId,
                                                            Description = t.Description,
                                                            Category = t.Category,
                                                            SubCategory = t.SubCategory,
                                                            TestCaseItemType = t.Type,
                                                            TestCaseType = t.Type, // TODO: подумать, какой здесь должен быть тип
                                                            TestMethods = new []{ $"{t.TestMethod.ReflectedType!.FullName}.{t.TestMethod.Name}" }
                                                        })
                                       .ToArray();

            // конкатим тест кейсы
            var testCases = declaredTestCases.Concat(inlineTestCases).ToArray();

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
        /// <param name="tests">Тесты</param>
        /// <returns>
        ///     Возвращает null, если создать экземпляр не удалось
        /// </returns>
        private TestCase? CreateTestCase(MethodInfo producerMethod, Test[] tests)
        {
            TestCaseItemBase? instance;
            if (producerMethod.IsStatic)
            {
                // пробуем инстанцировать тест кейс из статического вызова без аргументов
                instance = producerMethod.Invoke(null, parameters: null) as TestCaseItemBase;
                if (instance == null)
                    return null;
            }
            else
            {
                // пробуем инстанцировать тест кейс из экземплярного вызова без аргументов
                instance = producerMethod.Invoke(Activator.CreateInstance(producerMethod.ReflectedType!), parameters: null) as TestCaseItemBase;
                if (instance == null)
                    return null;
            }

            // тестовые методы, где используется тест кейс
            var methods = tests.Where(t => t.TheoryData.Any(d => d == producerMethod.ReflectedType!) ||
                                           t.CaseProducers.Contains(producerMethod))
                               .Select(t => $"{t.Method.ReflectedType!.FullName}.{t.Method.Name}")
                               .Distinct()
                               .ToArray();


            return new TestCase
            {
                TestId = instance.TestId,
                Description = instance.Description,
                Category = instance.Category,
                SubCategory = instance.SubCategory,
                TestMethods = methods,
                TestCaseItemType = producerMethod.ReturnType,
                TestCaseType = producerMethod.ReflectedType!
            };
        }

        /// <summary>
        ///     Возвращает все тесты определённые в типах
        /// </summary>
        /// <param name="types">Сканируемые типы</param>
        /// <param name="testCaseProducersMethods">Методы, которые производят тест кейсы</param>
        /// <param name="testCaseTypes">Типы тест кейсов</param>
        private Test[] GetTests(Type[] types, MethodInfo[] testCaseProducersMethods, Type[] testCaseTypes)
        {
            var tests = types
                        // выбираем все публичные методы
                        .SelectMany(type => type.GetMethods(BindingFlags.Public |
                                                            BindingFlags.InvokeMethod |
                                                            BindingFlags.Instance |
                                                            BindingFlags.Static))
                        // которые помечены как Fact или Theory
                        .Where(methodInfo => methodInfo.GetCustomAttribute<TheoryAttribute>() != null ||
                                             methodInfo.GetCustomAttribute<FactAttribute>() != null)
                        //.Where(m => m.Name == "Some_Test_With_Ctor")
                        // выбираем данные теста
                        .Select(m =>
                                    {
                                        // получаем инструкции тестового метода
                                        var instructions = GetMethodInstructions(m);
                                        return new Test
                                        {
                                            // тестовый метод
                                            Method = m,

                                            // вызовы методов, которые производят тест кейсы
                                            CaseProducers = GetProducersCalls(instructions, testCaseProducersMethods),

                                            // тест кейсы, которые конструируются в тесте через new
                                            InlineTestCases = GetInlineTestCases(instructions, m, testCaseTypes),

                                            // типы тест кейсов прописанных в теории 
                                            TheoryData = m.GetCustomAttributes(typeof(TestCaseDataAttribute))
                                                                     .Cast<TestCaseDataAttribute>()
                                                                     .Select(a => a.Class)
                                                                     .ToArray()
                                        };
                                    })
                        .ToArray();

            return tests;
        }

        /// <summary>
        ///     Для данного тестового метода возвращает вызовы методов, которые конструюруют тест кейсы
        /// </summary>
        /// <param name="instructions">Инструкции тестового метода</param>
        /// <param name="producers">Все методы, конструирующие тест кейсы</param>
        private MethodInfo[] GetProducersCalls(Instruction[] instructions, MethodInfo[] producers)
        {
            // получаем методы, конструирующие тест-кейсы в этом тесте
            var producersCalls = instructions.Select(i => i.Operand as MethodInfo)
                                             .Where(mi => mi != null)
                                             .Where(producers.Contains)
                                             .Select(mi => mi!)
                                             .ToArray();

            return producersCalls;
        }

        /// <summary>
        ///     Возвращает тест-кейсы, сконструированные прямо в тестовом методе
        /// </summary>
        /// <param name="instructions">Инструкции тестового метода</param>
        /// <param name="test">Тестовый метод</param>
        /// <param name="testCaseTypes">Типы тест-кейсов</param>
        private InlineTestCase[] GetInlineTestCases(Instruction[] instructions, MethodInfo test, Type[] testCaseTypes)
        {
            var results = new List<InlineTestCase>();

            foreach (var instruction in instructions)
            {
                if (instruction.OpCode != OpCodes.Newobj)
                    continue;

                if (instruction.Operand is MethodBase { MemberType: MemberTypes.Constructor } ctor)
                {
                    if (testCaseTypes.Contains(ctor.ReflectedType))
                    {
                        var testCase = GetInlineTestCase(ctor.ReflectedType!, test, instruction);
                        results.Add(testCase);
                    }
                }
            }

            return results.ToArray();
        }

        /// <summary>
        ///     Возвращает инструкции метода
        /// </summary>
        private Instruction[] GetMethodInstructions(MethodBase method)
        {
            // получем инструкции метода
            // для синхронного метода будем искать вызовы конструирования тест кейсов срели его инструкций
            var instructions = method.GetInstructions();

            // проверим, возможно метод асинхронный
            // берём первую инструкцию
            var firstInstruction = instructions[0];

            // если первая инструкция является вызовом конструктора
            if (firstInstruction.OpCode == OpCodes.Newobj)
            {
                // получаем вызываемый конструктор
                var ctor = (firstInstruction.Operand as MethodBase)!;

                // если конструируется класс наследник IAsyncStateMachine
                var asyncMethod = ctor.DeclaringType!.GetInterfaces().Any(i => i == typeof(IAsyncStateMachine));
                if (asyncMethod)
                {
                    // то понимаем, что тестовый метод был асинхронный и был развернут во вложенный класс, наследник IAsyncStateMachine
                    // c методами MoveNext и SetStateMachine

                    // берём метод MoveNext 
                    var moveNextMethod = ctor.DeclaringType.GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (moveNextMethod != null)
                    {
                        // будем искать вызовы конструирования тест кейсов в методе MoveNext
                        instructions = moveNextMethod.GetInstructions();
                    }
                }
            }

            return instructions.ToArray();
        }

        /// <summary>
        ///     Возвращает тест кейс, который конструируется в тесте через new
        /// </summary>
        /// <param name="testCaseType">Тип тест кейса</param>
        /// <param name="test">Тестовый метод</param>
        /// <param name="newObj">Инструкция new тест кейса</param>
        private InlineTestCase GetInlineTestCase(Type testCaseType, MethodInfo test, Instruction newObj)
        {
            var result = new InlineTestCase
            {
                TestMethod = test,
                Type = testCaseType
            };

            var currentInstruction = newObj;
            while (currentInstruction.OpCode.ToString().StartsWith("stloc") == false)
            {
                if (currentInstruction.OpCode == OpCodes.Callvirt)
                {
                    if (currentInstruction.Operand is MethodInfo setter)
                    {
                        switch (setter.Name)
                        {
                            case "set_TestId":
                                {
                                    var dup = currentInstruction.Previous;
                                    if (dup.OpCode == OpCodes.Ldstr)
                                    {
                                        result.TestId = dup.Operand as string;
                                    }
                                    break;
                                }
                            case "set_Description":
                                {
                                    var dup = currentInstruction.Previous;
                                    if (dup.OpCode == OpCodes.Ldstr)
                                    {
                                        result.Description = dup.Operand as string;
                                    }
                                    break;
                                }
                            case "set_Category":
                                {
                                    var dup = currentInstruction.Previous;
                                    if (dup.OpCode == OpCodes.Ldstr)
                                    {
                                        result.Category = dup.Operand as string;
                                    }
                                    break;
                                }
                            case "set_SubCategory":
                                {
                                    var dup = currentInstruction.Previous;
                                    if (dup.OpCode == OpCodes.Ldstr)
                                    {
                                        result.SubCategory = dup.Operand as string;
                                    }
                                    break;
                                }
                        }
                    }
                }

                currentInstruction = currentInstruction.Next;
            }

            return result;
        }

        /// <summary>
        ///     Тест
        /// </summary>
        private class Test
        {
            /// <summary>
            ///     Тестовый метод
            /// </summary>
            public MethodInfo Method { get; set; } = default!;

            /// <summary>
            ///     Вызовы методов для создания тест кейсов
            /// </summary>
            public MethodInfo[] CaseProducers { get; set; } = default!;

            /// <summary>
            ///     Тест кейсы, сконструированные в тесте напрямую
            /// </summary>
            public InlineTestCase[] InlineTestCases { get; set; } = default!;

            /// <summary>
            ///     Кейсы в теории
            /// </summary>
            public Type[] TheoryData { get; set; } = default!;
        }

        /// <summary>
        ///     Заинлайненный тест кейс
        /// </summary>
        private class InlineTestCase
        {
            /// <summary>
            ///     Тип тест кейса
            /// </summary>
            public Type Type { get; set; }

            /// <summary>
            ///     Тестовый метод
            /// </summary>
            public MethodInfo TestMethod { get; set; } = default!;

            /// <summary>
            ///     Идентификатор теста
            /// </summary>
            public string? TestId { get; set; }

            /// <summary>
            ///     Описание тест кейса
            /// </summary>
            public string? Description { get; set; }

            /// <summary>
            ///     Категория - для разделения кейсов на первом уровне
            /// </summary>
            public string? Category { get; set; }

            /// <summary>
            ///     Подкатегория - для разделения кейсов на втором уровне
            /// </summary>
            public string? SubCategory { get; set; }
        }
    }
}
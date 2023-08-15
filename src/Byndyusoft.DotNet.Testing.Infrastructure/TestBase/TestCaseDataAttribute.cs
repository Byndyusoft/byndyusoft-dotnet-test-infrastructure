namespace Byndyusoft.DotNet.Testing.Infrastructure.TestBase;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

/// <summary>
///     Предоставляет источник тестовых данных для параметризованного теста (<see cref="Xunit.TheoryAttribute"/>).
///     Тестовые данные берутся из класса, который должен реализовывать интерфейс <see cref="ITestCaseData{T}" />
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseDataAttribute : DataAttribute
{
    /// <summary>
    ///     Инициализирует <see cref="TestCaseDataAttribute" /> класс.
    /// </summary>
    /// <param name="testCaseType">
    ///     Класс, который предоставляет данные тестового сценария. Должен наследоваться от
    ///     <see cref="ITestCaseData{T}" />
    /// </param>
    public TestCaseDataAttribute(Type testCaseType) => Class = testCaseType;

    /// <summary>Получение типа класса, который возвращает тестовый сценарий</summary>
    public Type Class { get; }

    /// <inheritdoc />
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (Activator.CreateInstance(Class) is ITestCaseData<TestCaseItemBase> instance)
            return new[] { new[] { instance.Get() } };

        throw new ArgumentException($"{Class.FullName} must implement ITestCaseData<TestCaseItemBase> to be used as TestCaseData for the test method named '{testMethod.Name}' on {testMethod.DeclaringType!.FullName}");
    }
}
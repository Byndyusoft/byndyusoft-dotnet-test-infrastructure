namespace Byndyusoft.DotNet.Testing.Infrastructure.TestBase;

/// <summary>
///     Интерфейс данных одного тест кейса
/// </summary>
/// <typeparam name="T">Тип тест кейса</typeparam>
public interface ITestCaseData<out T> where T : TestCaseItemBase
{
    /// <summary>
    ///     Возвращает тест кейс (тестовый сценарий)
    /// </summary>
    T Get();
}
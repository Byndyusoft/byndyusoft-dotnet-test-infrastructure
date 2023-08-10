namespace Byndyusoft.DotNet.Testing.Infrastructure.TestBase
{
    /// <summary>
    ///     Интерфейс данных одного тест кейса
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITestCaseData<out T> where T : TestCaseItemBase
    {
        /// <summary>
        ///     Метод получения тестового сценария
        /// </summary>
        /// <returns></returns>
        T Get();
    }
}
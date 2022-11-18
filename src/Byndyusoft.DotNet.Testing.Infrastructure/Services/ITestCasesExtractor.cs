namespace Byndyusoft.DotNet.Testing.Infrastructure.Services
{
    using System.Reflection;

    /// <summary>
    ///     Служба извлечения тест-кейсов из сборок с авто-тестами
    /// </summary>
    public interface ITestCasesExtractor
    {
        /// <summary>
        ///     Возвращает тест-кейсы из переданных сборок
        /// </summary>
        /// <remarks>
        ///     При вызове из кода сборки с авто-тестами, вернёт все тест-кейсы этой в сборке
        /// </remarks>
        TestCase[] Get(params Assembly[] assemblies);
    }
}
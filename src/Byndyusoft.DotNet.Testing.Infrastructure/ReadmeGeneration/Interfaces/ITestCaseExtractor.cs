namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Interfaces
{
    using System.Reflection;
    using Entities;

    /// <summary>
    ///     Служба получения тест-кейсов из сборок с авто-тестами
    /// </summary>
    public interface ITestCaseExtractor
    {
        /// <summary>
        ///     Возвращает тест-кейсы из переданных сборок
        /// </summary>
        TestCase[] Get(params Assembly[] assemblies);
    }
}
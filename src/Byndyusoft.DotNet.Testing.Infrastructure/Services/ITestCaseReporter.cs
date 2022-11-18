namespace Byndyusoft.DotNet.Testing.Infrastructure.Services
{
    using System.Reflection;

    /// <summary>
    ///     Служба для формирования отчета по тест-кейсам
    /// </summary>
    public interface ITestCaseReporter
    {
        /// <summary>
        ///     Возвращает строкове представление отчёта по тесткейсам из переданных сборок
        /// </summary>
        string Build(params Assembly[] assemblies);
    }
}
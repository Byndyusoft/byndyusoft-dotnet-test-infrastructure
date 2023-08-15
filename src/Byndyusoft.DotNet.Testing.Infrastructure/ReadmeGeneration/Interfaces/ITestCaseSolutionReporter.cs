namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Interfaces;

using System.Reflection;
using System.Threading.Tasks;

/// <summary>
///     Служба создания отчёта по тест кейсам решения
/// </summary>
public interface ITestCaseSolutionReporter
{
    /// <summary>
    ///     Генерирует документ readme по тест кейсам решения
    /// </summary>
    /// <returns>
    ///     Возвращает true, если в отчёте есть ошибки
    /// </returns>
    Task<bool> Generate(params Assembly[] assemblies);
}
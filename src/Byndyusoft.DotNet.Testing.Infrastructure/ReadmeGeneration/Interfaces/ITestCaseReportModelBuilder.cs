namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Interfaces;

using System.Reflection;
using System.Threading.Tasks;
using Entities;

/// <summary>
///     Билдер объектной модели отчёта
/// </summary>
internal interface ITestCaseReportModelBuilder
{
    /// <summary>
    ///     Возвращает объектную модель отчёта по тест кейсам
    /// </summary>
    /// <param name="assemblies">Сборки для поиска искать тест кейсы</param>
    Task<ReadmeReport> Build(params Assembly[] assemblies);
}
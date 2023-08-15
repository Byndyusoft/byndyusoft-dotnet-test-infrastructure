namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Interfaces;

using System.Reflection;
using System.Threading.Tasks;

/// <summary>
///     Служба формирования отчёта по тест кейсам
/// </summary>
public interface ITestCaseReportBuilder
{
    /// <summary>
    ///     Возвращает строковое представление отчёта по тест кейсам из переданных сборок
    /// </summary>
    /// <returns>
    ///     Также возвращает флаг наличия ошибок в отчёте
    /// </returns>
    Task<(string, bool)> Build(params Assembly[] assemblies);
}
namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Interfaces;

using System.Reflection;
using System.Threading.Tasks;
using Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities;
using Services;

/// <summary>
///     Служба создания отчёта по тест кейсам решения
/// </summary>
public interface ITestCaseSolutionReporter
{
    /// <summary>
    ///     Добавляет в корень решения отчёт по тест кейсам
    /// </summary>
    /// <returns>
    ///     Возвращает статус конситентности отчёта
    /// </returns>
    Task<ReportConsistency> AddReport(params Assembly[] assemblies);
}
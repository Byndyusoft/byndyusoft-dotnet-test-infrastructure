namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Interfaces;

using System.Reflection;
using Entities;

/// <summary>
///     Служба получения тест-кейсов из сборок с авто-тестами
/// </summary>
public interface ITestCaseExtractor
{
    /// <summary>
    ///     Возвращает тест-кейсы из переданных сборок и ошибки формирования тест кейсов, если они были
    /// </summary>
    (TestCase[] TestCases, string? Errors) Get(params Assembly[] assemblies);
}
namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Отчёт по тест-кейсам
    /// </summary>
    internal sealed class ReadmeReport
    {
        /// <summary>
        ///     Имя отчёта - заголовок верхнего уровня
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        ///     Общее описание отчёта
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        ///     Общее количество тест-кейсов в отчёте
        /// </summary>
        public int TestCount => Categories.Sum(category => category.TestCount);

        /// <summary>
        ///     Категории тест-кейсов
        /// </summary>
        public ReadmeCategory[] Categories { get; set; } = default!;

        /// <summary>
        ///     Ошибки валидации шаблона
        /// </summary>
        public string? TemplateValidationErrors { get; set; }

        /// <summary>
        ///     Возвращает ошибки формирования отчёта
        /// </summary>
        public ReportErrors Validate()
        {
            // тест-кейсы
            var testCases = Categories.SelectMany(c => c.SubCategories.SelectMany(s => s.TestCases))
                                      .ToArray();

            // пустые идентификаторы
            var emptyIds = testCases.Where(t => string.IsNullOrWhiteSpace(t.TestId))
                                    .ToArray();


            // пустые описания
            var emptyDescriptions = testCases.Where(t => string.IsNullOrWhiteSpace(t.Description))
                                             .ToArray();

            // дублированыные идентификаторы
            var duplicateTestIds = testCases.GroupBy(t => t.TestId)
                                            .Where(t => string.IsNullOrWhiteSpace(t.Key) == false)
                                            .Where(g => g.Count() > 1)
                                            .ToDictionary(g => g.Key!, g => g.ToArray());

            return new ReportErrors
            {
                TemplateValidationErrors = TemplateValidationErrors,
                EmptyIds = emptyIds,
                EmptyDescriptions = emptyDescriptions,
                DuplicateTestIds = duplicateTestIds
            };
        }
    }

    /// <summary>
    ///     Ошибки формирования отчёта
    /// </summary>
    public class ReportErrors
    {
        /// <summary>
        ///     Ошибки валидации шаблона
        /// </summary>
        public string? TemplateValidationErrors { get; set; }

        /// <summary>
        ///     Тест кейсы с пустыми идентификаторами
        /// </summary>
        public TestCase[] EmptyIds { get; set; } = default!;

        /// <summary>
        ///     Тест кейсы с пустыми описаниями
        /// </summary>
        public TestCase[] EmptyDescriptions { get; set; } = default!;

        /// <summary>
        ///     Тест кейсы с дублированыными идентификаторами
        /// </summary>
        public Dictionary<string, TestCase[]> DuplicateTestIds { get; set; } = default!;

        /// <summary>
        ///     Возвращает true, если есть ошибки ошибки валидации шаблона
        /// </summary>
        public bool HasTemplateValidationErrors => string.IsNullOrWhiteSpace(TemplateValidationErrors) == false;
        /// <summary>
        ///     Возвращает true, если есть пустые идентификаторы тест-кейсов 
        /// </summary>
        public bool HasEmptyIds => EmptyIds.Any();
        /// <summary>
        ///     Возвращает true, если есть пустые описания тест кейсов 
        /// </summary>
        public bool HasEmptyDescriptions => EmptyDescriptions.Any();
        /// <summary>
        ///     Возвращает true, если есть дублирванные идентификаторы тест-кейсов 
        /// </summary>
        public bool HasDuplicateTestIds => DuplicateTestIds.Any();
        
        /// <summary>
        ///     Возвращает true, если есть ошибки 
        /// </summary>
        public bool HasErrors => HasTemplateValidationErrors ||
                                 HasEmptyIds ||
                                 HasEmptyDescriptions ||
                                 HasDuplicateTestIds;
    }
}
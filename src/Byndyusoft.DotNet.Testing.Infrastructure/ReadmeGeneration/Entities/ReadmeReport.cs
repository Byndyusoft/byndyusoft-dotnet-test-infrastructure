namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities;

    using System.Linq;

    /// <summary>
    ///     Отчёт по тест-кейсам
    /// </summary>
    internal sealed class ReadmeReport
    {
        /// <summary>
        ///     Имя отчёта - заголовок верхнего уровня
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Общее описание отчёта
        /// </summary>
        public string? Description { get; }

        /// <summary>
        ///     Категории тест-кейсов
        /// </summary>
        public ReadmeCategory[] Categories { get; }

        /// <summary>
        ///     Ошибки валидации шаблона отчёта
        /// </summary>
        private readonly string? _templateValidationErrors;

        /// <summary>
        ///     Ошибки формирования тест кейсов
        /// </summary>
        private readonly string? _testCaseErrors;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="name">Имя отчёта</param>
        /// <param name="description">Общее описание</param>
        /// <param name="categories">Категории тесткейсов</param>
        /// <param name="templateValidationErrors">Ошибки валидации шаблона</param>
        /// <param name="testCaseErrors">Ошибки формирования тест кейсов</param>
        public ReadmeReport(string name, 
                            string? description, 
                            ReadmeCategory[] categories, 
                            string? templateValidationErrors, 
                            string? testCaseErrors)
        {
            Name = name;
            Description = description;
            Categories = categories;
            _templateValidationErrors = templateValidationErrors;
            _testCaseErrors = testCaseErrors;
        }

        /// <summary>
        ///     Общее количество тест-кейсов в отчёте
        /// </summary>
        public int TestsCount => Categories.Sum(category => category.TestsCount);

        /// <summary>
        ///     Возвращает ошибки формирования отчёта
        /// </summary>
        public ReportErrors GetErrors()
        {
            // собираем все тест-кейсы
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
                TestCaseErrors = _testCaseErrors,
                TemplateValidationErrors = _templateValidationErrors,
                EmptyIds = emptyIds,
                EmptyDescriptions = emptyDescriptions,
                DuplicateTestIds = duplicateTestIds
            };
        }
    }

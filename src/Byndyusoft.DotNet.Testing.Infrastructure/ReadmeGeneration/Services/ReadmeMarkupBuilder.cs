namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Services;

using System.Linq;
using System.Text;
using Byndyusoft.DotNet.Testing.Infrastructure.Extensions;
using Entities;

/// <summary>
///     Билдер разметки отчёта
/// </summary>
internal sealed class ReadmeMarkupBuilder
{
    private const string TableOfContentHeader = "# Содержание:";
    private const string NotAutomatedLabel = "Не автоматизирован";

    /// <summary>
    ///     Оглавление
    /// </summary>
    private readonly StringBuilder _tableOfContent;

    /// <summary>
    ///     Тело отчёта
    /// </summary>
    private readonly StringBuilder _body;

    /// <summary>
    ///     Ссылка на раздел с общим описание отчёта
    /// </summary>
    private readonly string _name;

    /// <summary>
    ///     Общее описание отчёта
    /// </summary>
    private readonly string? _description;

    /// <summary>
    ///     Ссылка на раздел с общим описание отчёта
    /// </summary>
    private readonly string _descriptionLink;

    /// <summary>
    ///     Если в отчёте только пустые категории
    /// </summary>
    private readonly bool _hasOnlyEmptyCategories;

    /// <summary>
    ///     Общее число тестов в отчёте
    /// </summary>
    private readonly int _testsCount;

    /// <summary>
    ///     Ошибки формирования отчёта
    /// </summary>
    private readonly ReportErrors _errors;

    /// <summary>
    ///     True, если у отчёта есть общее описание
    /// </summary>
    private bool ReportHasDescription => string.IsNullOrWhiteSpace(_description) == false;

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="readmeReport">Отчёт по тест кейсам</param>
    public ReadmeMarkupBuilder(ReadmeReport readmeReport)
    {
        // метка ссылки на общее описание тесткейсов
        _descriptionLink = "root-readme-description-link-should-never-duplicate";
            
        _description = readmeReport.Description;
        _name = readmeReport.Name;
        _testsCount = readmeReport.TestsCount;
        _errors = readmeReport.GetErrors();

        // есть ли в отчёте непустые категории 
        _hasOnlyEmptyCategories = readmeReport.Categories.All(c => c.Name is null);

        // формируем содержание readme
        _tableOfContent = new StringBuilder(TableOfContentHeader);
        _tableOfContent.AppendLine("  ");
        _tableOfContent.AppendLine("  ");

        // добавляем в содержание ссылку на общее описание тесткейсов, если оно определено в шаблоне
        if (ReportHasDescription)
            _tableOfContent.AppendLine($"* [Описание](#{_descriptionLink})");

        // формируем основное тело с описанием тест кейсов
        _body = new StringBuilder();

        // добавляем раздел "Условные обозначения"
        AddLegend();
    }

    /// <summary>
    ///     Добавляет в разметку отчёта данные категоии
    /// </summary>
    /// <param name="category">Категория</param>
    public void AddCategory(ReadmeCategory category)
    {
        if (_hasOnlyEmptyCategories)
            return;

        // добавляем ссылку на категорию в содержание
        var categoryName = category.Name ?? "NoCategory";

        // добавляем ссылку на категорию в содержание
        _tableOfContent.AppendLine($"* [{categoryName} (Тестов: {category.TestsCount})](#{categoryName.ToBase64()})");

        // добавляем категорию в тело
        _body.AppendLine(@$"<a id=""{categoryName.ToBase64()}""></a>");
        _body.AppendLine($"## {categoryName} (Тестов: {category.TestsCount})");
            
        // добавляем описание категории, если оно определено в шаблоне
        if (category.Description != null)
            _body.AppendLine(category.Description);
        _body.AppendLine();
    }

    /// <summary>
    ///     Добавляет в разметку отчёта данные категоии
    /// </summary>
    /// <param name="category">Категория</param>
    /// <param name="subCategory">Подкатегория</param>
    public void AddSubCategory(ReadmeCategory category, ReadmeSubCategory subCategory)
    {
        // есть ли в категории непустые подкатегории 
        var hasOnlyEmptySubCategories = category.SubCategories.All(c => c.Name is null);
        if (hasOnlyEmptySubCategories)
            return;

        // имя категории
        var categoryName = category.Name ?? "NoCategory";

        // имя подкатегории
        var subCategoryName = subCategory.Name ?? "NoSubCategory";

        // добавляем ссылку на подкатегорию в содержание
        _tableOfContent.AppendLine($"  * [{subCategoryName} (Тестов: {subCategory.TestsCount})](#{categoryName.ToBase64()}-{subCategoryName.ToBase64()})");

        // добавляем подкатегорию в тело
        _body.AppendLine(@$"<a id=""{categoryName.ToBase64()}-{subCategoryName.ToBase64()}""></a>");
        _body.AppendLine($"### {subCategoryName} (Тестов: {subCategory.TestsCount})");
        // добавляем описание подкатегории, если оно определено в шаблоне
        if (subCategory.Description != null)
            _body.AppendLine(subCategory.Description);
        _body.AppendLine();
    }

    /// <summary>
    ///     Добавляет в разметку отчёта данные категоии
    /// </summary>
    /// <param name="testCase">Тест кейс</param>
    public void AddTestCase(TestCase testCase)
    {
        // получаем краткое имя тест кейса из первой строки описания 
        var testName = testCase.Name ?? "<ТЕСТ-КЕЙС БЕЗ НАЗВАНИЯ>";
        testName = testName.Replace("\r", "");
        var automationLabel = testCase.IsAutomated ? "(A)" : "(P)";
        testName = $"{automationLabel} {testName}";

        // получаем идентификатор тест кейса
        var testId = string.IsNullOrWhiteSpace(testCase.TestId) ? "<ТЕСТ-КЕЙС БЕЗ ИД>" : testCase.TestId;

        // получаем тестовые методы, которые используют тест кейс
        var testMethods = testCase.IsAutomated ? testCase.TestMethods : new[] { NotAutomatedLabel };

        // добавляем ссылку на тест кейс в содержание
        var intend = _hasOnlyEmptyCategories ? "" : "    ";
        _tableOfContent.AppendLine($"{intend}* [{testName}](#{testCase.TestCaseType.FullName})  ");

        // добавляем описание тест кейса в тело
        _body.AppendLine(@$"<a id=""{testCase.TestCaseType.FullName}""></a>");
        _body.AppendLine($"#### {testName}");
        _body.AppendLine("  ");
        _body.AppendLine($"**TestId:** {testId}  ");
        _body.AppendLine($"**TestClass:** {testCase.TestCaseType.FullName}  ");
        _body.AppendLine($"**TestMethods:** {string.Join(", ", testMethods)}");
        _body.AppendLine("  ");
        _body.AppendLine("**Description:**  ");
        _body.AppendLine($"{testCase.Description}");
        _body.AppendLine("---");
    }

    /// <summary>
    ///     Возвращает разметку отчёта
    /// </summary>
    public string Build()
    {
        // собираем readme
        var readme = new StringBuilder($"# {_name}")
                     .AppendLine()
                     .AppendLine($"#### Всего тестов: {_testsCount}")
                     .AppendLine("  ")
                     .AppendLine("  ");

        // добавляем ошибки
        if (_errors.HasErrors)
        {
            AddErrors();
        }

        // добавляем содержание
        readme.AppendLine(_tableOfContent.ToString());
        readme.AppendLine("---");

        // добавляем ссылку на общее описание, если оно определено в шаблоне
        if (ReportHasDescription)
        {
            readme.AppendLine(@$"<a id=""{_descriptionLink}""></a>");
            readme.AppendLine("# Описание");
            readme.AppendLine(_description);
            readme.AppendLine("---");
        }

        // добавляем основное тело с описанием тесткейсов
        readme.Append(_body.ToString());

        return readme.ToString();
    }

    /// <summary>
    ///     Добавляет раздел "Условные обозначения"
    /// </summary>
    private void AddLegend()
    {
        var legendLink = "readme-report-legend-link-should-never-duplicate";
        _tableOfContent.AppendLine($"* [Условные обозначения](#{legendLink})");
        _body.AppendLine(@$"<a id=""{legendLink}""></a>");
        _body.AppendLine("# Условные обозначения");

        _body.AppendLine("**Наименование тест-кейсов:**  ");
        _body.AppendLine("**(А)** - Авто-тест. Тест-кейс автоматизирован (для тест-кейса есть тестовый метод)   ");
        _body.AppendLine("**(Р)** - Ручной тест. Тест-кейс не автоматизирован (для тест-кейса нет тестового метода)   ");
        _body.AppendLine("  ");
        _body.AppendLine("**Данные тест-кейсов:**  ");
        _body.AppendLine(@"**TestId** - Идентификатор тест-кейса.  
**TestClass** - Название класса тест-кейса.  
**TestMethods** - Тестовые методы, в которых используется класс тест-кейса.  
**Description** - Описание тест-кейса. Из описания берётся первая непустая строка, которая используется в качестве названия тест-кейса.");
            
        _body.AppendLine("");
        _body.AppendLine("---");
    }

    /// <summary>
    ///     Добавляет ошибки в оглавление и тело отчёта
    /// </summary>
    private void AddErrors()
    {
        var errorsLink = "building-readme-report-errors-link-should-never-duplicate";
        _tableOfContent.AppendLine($"* [Ошибки формирования readme](#{errorsLink})");
        _body.AppendLine(@$"<a id=""{errorsLink}""></a>");
        _body.AppendLine("# Ошибки формирования readme");

        if (_errors.HasTestCaseErrors)
        {
            _body.AppendLine("## Ошибки формирования тест кейсов:");
            _body.AppendLine(_errors.TestCaseErrors);
        }

        if (_errors.HasTemplateValidationErrors)
        {
            _body.AppendLine("## Ошибки валидации шаблона:");
            _body.AppendLine(_errors.TemplateValidationErrors);
        }

        if (_errors.HasEmptyIds)
        {
            _body.AppendLine("## Пустые идентификаторы в тест кейсах:");
            foreach (var testCase in _errors.EmptyIds)
            {
                _body.AppendLine($"* {testCase.TestCaseType.FullName}");
            }
        }

        if (_errors.HasEmptyDescriptions)
        {
            _body.AppendLine("## Пустые описания в тест кейсах:");
            foreach (var testCase in _errors.EmptyDescriptions)
            {
                _body.AppendLine($"* {testCase.TestCaseType.FullName}");
            }
        }

        if (_errors.HasDuplicateTestIds)
        {
            _body.AppendLine("## Задублированные идентификаторы:");
            foreach (var item in _errors.DuplicateTestIds)
            {
                _body.AppendLine($"* TestId: {item.Key}");
                foreach (var testCase in item.Value)
                {
                    _body.AppendLine($"  * {testCase.TestCaseType.FullName}");
                }
            }
        }
    }
}
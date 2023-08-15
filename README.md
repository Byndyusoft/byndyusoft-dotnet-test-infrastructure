# Byndyusoft.DotNet.Testing.Infrastructure

Проект содержит вспомогательные объекты для использования в автоматических тестах.  

## Содержание:  
  
* [Инфраструктрура интеграционных тестов](#create-tests)  
* [Генератор отчёта по тест-кейсам](#TestCaseReadmeSolutionReporter)

---
<a name="create-tests"></a>
## Создание тестовых сценариев интеграционных тестов
Необходимо создать вспомогательные классы для класса тестового сценария

```csharp

    public class TestCaseParameters
    {
        ///Класс со всеми входными параметрами
    }

    public class TestCaseMocks
    {
        ///Класс с необходимыми результатами моков
    }

    public class TestCaseExpectations
    {
        ///Класс с необходимыми ожидаемыми результатами (строка в БД, сообщения в очередях, вызовы апи)
    }

```

Затем необходимо создать сам класс тестового сценария

```csharp

    public class TestCaseItem : TestCaseItemBase
    {
        /// <summary>
        ///     Название папки, где находится файл для обработки
        /// </summary>
        public string TestCaseFolder { get; set; }

        /// <summary>
        ///     Входные параметры
        /// </summary>
        public TestCaseParameters Parameters { get; set; }

        /// <summary>
        ///     Настройки моков для тестов
        /// </summary>
        public TestCaseMocks Mocks { get; set; }

        /// <summary>
        ///     Объект с ожидаемыми значениями, которые проверяются в тест-кейсе
        /// </summary>
        public TestCaseExpectations Expected { get; set; }
    }
    
   ```

   Теперь мы готовы заводить сами данные тестовых сценариев

   ```csharp
   public class BankStatementDocumentTestCase : ITestCaseData<TestCaseItem>
    {
        public TestCaseItem Get()
        {        
            return new TestCaseItem
                       {
                           TestId = "BankStatementDocument",
                           Description = @"BankStatementDocument",
                           TestCaseFolder = "BankStatementDocument",
                           Mocks = new TestCaseMocks
                                       {
                                           ///Данные моков
                                       },
                           Parameters = new TestCaseParameters
                                            {
                                                ///Входные данные
                                            },
                           Expected = new TestCaseExpectations
                                          {
                                              //Ожидаемые значения
                                          }
                       };
        }
   ```
   
   После подготовки всех необходимых данныех создаем тест-теорию

   ```csharp
        [Theory]
        [TestCaseData(typeof(BankStatementDocumentTestCase))]
        [TestCaseData(typeof(FormalizedXbrlDocumentTestCase))]
        [TestCaseData(typeof(FormalizedDepositoryDocumentTestCase))]
        public async Task ProcessQueueMessage_FileAndServiceSettings_AllArtifactsAreCorrect(TestCaseItem testCase)
        {
            _output.WriteLine(testCase.ToStringDescription());

            ///Основная логика теста
        }
   ```
  
---  
  
<a name="TestCaseReadmeSolutionReporter"></a>
## Генератор отчёта по тест-кейсам

**Назначение:** Формирование отчёта по всем тест-кейсам в проекте.  
**Формат:** Отчёт форимруется в markdown-формате.  
**Расположение:** Корневая директория решения.  
**Название файла:** `README_TestCases.md`  

Все тест-кейсы группируются по категориям/подкатегориям, если они указаны для тест-кейсов.  
В отчёте указано общее количество тест-кейсов, а также для каждой категории/подкатегории.  

Для тест-кейса отображается следующая информация:  
 - **TestId** - Идентификатор тест-кейса.  
 - **TestClass** - Название класса тест-кейса.  
 - **TestMethods** - Тестовые методы, в которых используется класс тест-кейса.  
 - **Description** - Описание тест-кейса. Из описания берётся первая непустая строка, которая используется в качестве названия тест-кейса.

**Условные обозначения для наименования тест-кейсов:**  
**(А)** - Авто-тест. Тест-кейс автоматизирован (для тест-кейса есть тестовый метод)   
**(Р)** - Ручной тест. Тест-кейс не автоматизирован (для тест-кейса нет тестового метода)   

**Требования к тест-кейсам:**  
Для того, чтобы тест-кейсы попадали в отчёт, они должны быть оформлены в виде классов, унаследованных от базового класса тест-кейсов `TestCaseItemBase` и создаваться через публичный метод `Get()` без параметров в отдельном классе.


```csharp

public class TestCaseItem : TestCaseItemBase
{

}

public class SomeTestCase1
{
    public TestCaseItem Get()
    {
        return new TestCaseItem
        {
            TestId = "TestId1",
            Description = @"

TestName1

Description of test-case 1  
bla-bla
",
            Category = "CategorySomeFeature",
            SubCategory = "SubCategorySomeScenarios"
        };
    }
}
```

### Inline тест кейсы

Генератор также сможет добавить в документацию заинлайненные тест кейсы. Такие тест кейсы создаются прямо в коде теста через конструкцию new и инициализатор свойств:

```csharp
[Fact]
public async Task Some_Important_Test()
{
    // ARRANGE
    var testCase = new TestCaseItem
    {
        TestId = "TestId1",
        Description = @"
TestName1

Description of test-case 1  
bla-bla
",
         Category = "CategorySomeFeature",
         SubCategory = "SubCategorySomeScenarios"
     }

    // ACT
        // что-то происходит

    // ASSERT
        // что-то проверяется
}
```
Для того, чтобы заинлайненный тест был корректно задокументирован, необходимо использовать литералы в инициализаторе, как показано в примере.


### Пример использования генератора:

Для создания отчёта установите в проекте пакет `Byndyusoft.DotNet.Testing.Infrastructure`  и создайте модульный тест примерно следующего содержания:

```csharp

[Fact]
public async Task Generate_FromCurrentDomain_ShouldGenerateReadmeInSolutionRoot()
{
    // ARRANGE
    var reporter = TestCaseReadmeSolutionReporter.New();

    // ACT
    var hasErrors = await reporter.Generate(Assembly.GetExecutingAssembly());

    // ASSERT
    hasErrors.Should().BeFalse();
}

```
  
После успешного выполнения данного теста в корневой директории решения должен находиться файл отчёта `README_TestCases.md`.

### Настройка отчёта с помощью шаблона

Генератор отчётов позволяет настроаивать содержимое отчета с помощью файла-шаблона.
Шаблон представляет собой md-файл с текстом в формате markdown.  
Шаблон может содержать Название - в заголовке первого уровня (#).  
После Названия можно укзать произвольный markdown-текст, который будет выведен сразу после содержания отчета  в разделе Описание.  
Также в шаблоне можно задать порядок сортировки и описание для Категорий/подкатегорий тест-кейсов.  

**Пример файла-шаблона:**  

Указанные в шаблоне категории будут идти в заданном порядке, а все остальные отсортированы по алфавиту.
  
```markdown
# Тест-кейсы
---
**Условные обозначения:**  
TODO  
**Примечания:**  
TODO  
---

## CategorySomeFeature
Важная фича

### SubCategorySomeScenarios
Сценарии важной фичи

## CategoryAnotherFeature
Другая важная фича

### Какая-то подкатегория
Сценарии другой важной фичи
```

**Пример генерации отчёта, с использованием шаблона:**  

```csharp

[Fact]
public async Task Generate_FromCurrentDomain_WithTemplate_ShouldGenerateReadmeInSolutionRoot()
{
    // ARRANGE
    // Настройки генератора отчёта
    var options = new TestCaseMarkdownReportingOptions
    {
        // Путь к файлу шаблона
        TemplatePath = Path.Combine("ReadmeGeneration", "TestCases", "WithTemplate", "README_TestCases_Template.md")
    };

    // В качестве параметра передаётся объект настроек где указан путь к шаблону
    var reporter = TestCaseReadmeSolutionReporter.New(options);

    // ACT
    var hasErrors = await reporter.Generate(Assembly.GetExecutingAssembly());

    // ASSERT
    hasErrors.Should().BeFalse();
}

```
  
### Ошибки формирования отчёта  

Если во время формирования отчёта произошли ошибки, например, ошибки валидации тест-кейсов (не заполнены ИД или описание, данные дублируются для разных тестов и т.п.), то в отчёте ошибки будут выведены в секции `Ошибки формирования readme`.  

### Примеры

Примеры генерации отчёта по тест кейсам можно найти в проекте с тестами в текущем решении - [TestCaseReadmeSolutionReporterTests](tests/Byndyusoft.DotNet.Testing.Infrastructure.Tests/ReadmeGeneration/TestCaseReadmeSolutionReporterTests.cs)  
Пример сгенерированного отчёта - [README_TestCases.md](README_TestCases.md)
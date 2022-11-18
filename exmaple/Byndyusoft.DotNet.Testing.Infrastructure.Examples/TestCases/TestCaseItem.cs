namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples.TestCases
{
    using global::Byndyusoft.DotNet.Testing.Infrastructure.TestBase;

    /// <summary>
    ///     Пример типа с данными тест-кейсов
    /// </summary>
    public class TestCaseItem : TestCaseItemBase
    {
        /// <summary>
        ///     Ожидаемые данные для проверок
        /// </summary>
        public TestEntity Expected { get; set; }
    }
}
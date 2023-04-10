namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Examples.TestCases
{
    using System;

    /// <summary>
    ///     Тестовая сущность с полями типа DateTimeOffset
    /// </summary>
    public class TestEntityWithDateTimeOffset
    {
        /// <summary>
        ///     Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Дата создания
        /// </summary>
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        ///     Дата изменения
        /// </summary>
        public DateTimeOffset UpdateDate { get; set; }
    }
}
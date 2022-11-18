namespace Byndyusoft.DotNet.Testing.Infrastructure.Services
{
    using System;
    using Byndyusoft.DotNet.Testing.Infrastructure.TestBase;

    /// <summary>
    ///     Тест кейс
    /// </summary>
    public class TestCase
    {
        /// <summary>
        ///     Идентификатор тест кейса
        /// </summary>
        public string TestId { get; set; } = default!;

        /// <summary>
        ///     Описание тест кейса
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        ///     Тип, который продуцирует экземпляр тест кейса
        /// </summary>
        public Type? ProducerType { get; set; }

        /// <summary>
        ///     Тип тест кейса, наследник от <see cref="TestCaseItemBase"/>
        /// </summary>
        public Type TestCaseType { get; set; } = default!;

        /// <summary>
        ///     True, если тест кейс задублирован по идентификатору на множестве кейсов
        /// </summary>
        public bool Duplicated { get; set; }
    }
}
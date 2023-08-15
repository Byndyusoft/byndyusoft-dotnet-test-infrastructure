namespace Byndyusoft.DotNet.Testing.Infrastructure.ReadmeGeneration.Entities
{
    /// <summary>
    ///     Категория тест кейсов
    /// </summary>
    internal sealed class ReadmeCategory
    {
        /// <summary>
        ///     Имя категории
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        ///     Описание категории
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        ///     Общее количество тест-кейсов в категории
        /// </summary>
        public int TestsCount { get; set; }

        /// <summary>
        ///     Порядковая метка категории в шаблоне
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        ///     Подкатегории тест-кейсов
        /// </summary>
        public ReadmeSubCategory[] SubCategories { get; set; } = default!;
    }
}
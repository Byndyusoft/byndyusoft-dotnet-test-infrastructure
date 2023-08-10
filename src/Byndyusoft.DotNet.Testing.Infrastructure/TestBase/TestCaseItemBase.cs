namespace Byndyusoft.DotNet.Testing.Infrastructure.TestBase
{
    using Newtonsoft.Json;
    using Xunit.Abstractions;

    /// <summary>
    ///     Базовый класс для TestCaseItem.
    ///     Даёт возможность запускать конкретный тест из списка параметризованного теста
    /// </summary>
    public abstract class TestCaseItemBase : IXunitSerializable
    {
        /// <summary>
        ///     Идентификатор тест-кейса
        /// </summary>
        public string TestId { get; set; }

        /// <summary>
        ///     Описание тест-кейса
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Категория - для разделения кейсов на первом уровне
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        ///     Подкатегория - для разделения кейсов на втором уровне
        /// </summary>
        public string? SubCategory { get; set; }

        public void Deserialize(IXunitSerializationInfo info)
        {
            var serialized = info.GetValue<string>(nameof(TestCaseItemBase));

            var testCaseItem = JsonConvert.DeserializeObject(
                                                             serialized,
                                                             new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                                                            );
            DeepCopy(testCaseItem, this);
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(
                          nameof(TestCaseItemBase),
                          JsonConvert.SerializeObject(
                                                      this,
                                                      new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                                                     )
                         );
        }

        public override string ToString()
        {
            return TestId;
        }

        /// <summary>
        ///     Возвращает строковое представление метаданных тест-кейса
        /// </summary>
        /// <returns>Строковое представление метаданных тест-кейса</returns>
        public string ToStringDescription()
        {
            return $@"
TestId: {TestId}
Description:
{Description}
";
        }

        private static void DeepCopy<T>(T from, T to)
        {
            foreach (var propertyInfo in from.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(from);
                propertyInfo.SetValue(to, value);
            }
        }
    }
}
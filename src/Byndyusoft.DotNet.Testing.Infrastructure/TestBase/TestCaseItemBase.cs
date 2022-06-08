namespace Byndyusoft.DotNet.TestInfrastructure.TestBase
{
    using Newtonsoft.Json;
    using Xunit.Abstractions;

    /// <summary> 
    /// Базовый класс для TestCaseItem. 
    /// Даёт возможность запускать конкретный тест из списка параметризованного теста 
    /// </summary> 
    public abstract class TestCaseItemBase : IXunitSerializable
    {
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

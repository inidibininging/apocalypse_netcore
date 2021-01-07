using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Newtonsoft.Json;
using System;

namespace Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter
{
    public class JsonSerializerAdapter : IStringSerializationAdapter
    {

        public T DeserializeObject<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public object DeserializeObject(string content, Type type)
        {
            return JsonConvert.DeserializeObject(content,type);
        }

        public string SerializeObject<T>(T instance)
        {
            return JsonConvert.SerializeObject(instance);
        }

        public string SerializeObject(object instance, Type type)
        {
            return JsonConvert.SerializeObject(instance);
        }
    }
}

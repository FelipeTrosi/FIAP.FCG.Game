using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FIAP.FCG.Game.Service.Util;

public static class ParseModel
{
    public static TDestination Map<TDestination>(this object origem, bool usePropertyName = false)
    {
        JsonSerializerSettings settings = new();

        settings.PreserveReferencesHandling = PreserveReferencesHandling.All;

        if (usePropertyName)
            settings.ContractResolver = new PropertyNameContractResolver();

        var serialized = JsonConvert.SerializeObject(origem, Formatting.Indented, settings);

        return JsonConvert.DeserializeObject<TDestination>(serialized, settings) ?? throw new Exception($"A Map do objeto {nameof(origem)} retornou null value");
    }
}

public class PropertyNameContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);

        foreach (JsonProperty prop in list)
            prop.PropertyName = prop.UnderlyingName;

        return list;
    }
}

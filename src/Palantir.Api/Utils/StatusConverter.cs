using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Palantir.Api.Utils
{
	public class StatusConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(string) || objectType == typeof(object);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
			{
				return reader.Value.ToString();
			}
			else if (reader.TokenType == JsonToken.StartObject)
			{
				var jObject = JObject.Load(reader);
				return jObject["status"]?.Value<string>() ?? "";
			}

			return "";
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is string statusString)
			{
				writer.WriteValue(statusString);
			}
			else
			{
				writer.WriteNull();
			}
		}
	}

}

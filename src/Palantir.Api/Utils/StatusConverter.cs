using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Palantir.Api.Utils
{
	/// <summary>
	/// Converts a ClickUp status to a string.
	/// Dealing with property "status" been an object when creating task and been an string when updating task.
	/// </summary>
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

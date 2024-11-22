using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Palantir.Api.Utils
{
	/// <summary>
	/// Converts a ClickUp priority to an integer.
	/// Dealing with property "priority" been an object when creating task and been an int when updating task.
	/// </summary>
	public class PriorityConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(int) || objectType == typeof(object);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Integer)
			{
				return Convert.ToInt32(reader.Value);
			}
			else if (reader.TokenType == JsonToken.StartObject)
			{
				var jObject = JObject.Load(reader);
				return jObject["id"]?.Value<int>() ?? 0;
			}

			return 0;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is int priorityInt)
			{
				writer.WriteValue(priorityInt);
			}
			else
			{
				writer.WriteNull();
			}
		}
	}

}

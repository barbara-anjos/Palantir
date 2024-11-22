using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Palantir.Api.Utils;

namespace Palantir.Api.Models
{
	/// <summary>
	/// Represents a ClickUp task model.
	/// </summary>
	public class ClickUpTaskModel
	{
		public class ClickUpTask
		{
			[JsonProperty("id")]
			public string? Id { get; set; }

			[JsonProperty("name")]
			public string Name { get; set; }

			[JsonProperty("description")]
			public string Description { get; set; }

			[JsonProperty("start_date")]
			public long? StartDate { get; set; }

			[JsonProperty("due_date")]
			public long? DueDate { get; set; }

			[JsonProperty("time_estimate")]
			public long? TimeEstimate { get; set; }

			[JsonProperty("priority")]
			[JsonConverter(typeof(PriorityConverter))]
			public int? Priority { get; set; }

			[JsonProperty("status")]
			[JsonConverter(typeof(StatusConverter))]
			public string? Status { get; set; }

			[JsonProperty("tags")]
			public List<Tags>? Tags { get; set; }

			[JsonProperty("custom_fields")]
			public List<ClickUpCustomField>? CustomFields { get; set; }
		}

		/// <summary>
		/// Represents the tags of a ClickUp task.
		/// </summary>
		public class Tags
		{
			[JsonProperty("name")]
			public string? Name { get; set; }
		}

		/// <summary>
		/// Represents the priority of a ClickUp task.
		/// </summary>
		public class Priority
		{
			[JsonProperty("id")]
			public string? Id { get; set; }
		}

		/// <summary>
		/// Represents the Custom Fields of a ClickUp task.
		/// </summary>
		public class ClickUpCustomField
		{
			public ClickUpCustomField() { }

			[JsonProperty("id")]
			public string? Id { get; set; }

			[JsonProperty("name")]
			public string? Name { get; set; }

			[JsonProperty("type")]
			public string? Type { get; set; }

			[JsonProperty("value")]
			private JToken? RawValue { get; set; }

			[JsonIgnore]
			public object? Value
			{
				get
				{
					return Type switch
					{
						"labels" => RawValue?.ToObject<List<string>>(),
						_ => RawValue?.ToString()
					};
				}
				set
				{
					RawValue = value is List<string> list ? JToken.FromObject(list) : JToken.FromObject(value);
				}
			}

			[JsonProperty("required")]
			public bool Required { get; set; }
		}

		public class TaskList
		{
			public List<ClickUpTask> Tasks { get; set; }
		}
	}
}

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static Palantir.Api.Models.ClickUpTaskModel;

namespace Palantir.Api.Models
{
    public class ClickUpTaskModel
    {
        public class ClickUpWebhookRequest
        {
            public string Event { get; set; }
            public ClickUpTask Task { get; set; }
            public string WorkspaceId { get; set; }
            public string TeamId { get; set; }
            public DateTime Timestamp { get; set; }
            public string ObjectId => Task?.Id;
        }

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

			//[JsonProperty("priority")]
			public int? Priority { get; set; }

			[JsonProperty("tags")]
			public List<Tags>? Tags { get; set; }

			[JsonProperty("custom_fields")]
			public List<ClickUpCustomField>? CustomFields { get; set; }
        }

		public class Tags
		{
			public string Name { get; set; }
		}


		public class ClickUpCustomField
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }

        public class ClickUpTaskResponse
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public List<ClickUpCustomField> CustomFields { get; set; }
            public string DueDate { get; set; }
            public string Assignee { get; set; }
        }

		public class ClickUpTaskUpdateData
		{
			public string Status { get; set; }
			public string Priority { get; set; }
			public DateTime DueDate { get; set; }
		}

		public class TaskList
		{
			public List<ClickUpTask> Tasks { get; set; }
		}

	}
}

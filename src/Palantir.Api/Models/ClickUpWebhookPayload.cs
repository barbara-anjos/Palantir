using Newtonsoft.Json;
using static Palantir.Api.Models.ClickUpTaskModel;

namespace Palantir.Api.Models
{
	public class ClickUpWebhookPayload
	{
		[JsonProperty("event")]
		public string Event { get; set; }

		[JsonProperty("history_items")]
		public HistoryItems HistoryItems { get; set; }

		[JsonProperty("task_id")]
		public string TaskId { get; set; }
	}

	public class HistoryItems
	{
		[JsonProperty("field")]
		public string Field { get; set; }

		[JsonProperty("after")]
		public After After { get; set; }
	}

	public class After
	{
		[JsonProperty("priority")]
		public string? Priority { get; set; }

		[JsonProperty("status")]
		public string? Status { get; set; }
	}
}

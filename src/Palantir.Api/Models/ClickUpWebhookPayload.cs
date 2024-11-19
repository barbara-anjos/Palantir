using Newtonsoft.Json;
using static Palantir.Api.Models.ClickUpTaskModel;

namespace Palantir.Api.Models
{
	public class ClickUpWebhookPayload
	{
		[JsonProperty("event")]
		public string Event { get; set; }

		//[JsonProperty("history_items")]
		public HistoryItems[] History_items { get; set; }

		//[JsonProperty("task_id")]
		public string Task_id { get; set; }
	}

	public partial class HistoryItems
	{
		[JsonProperty("field")]
		public string Field { get; set; }

		[JsonProperty("after")]
		public After After { get; set; }
	}

	public partial class After
	{
		[JsonProperty("priority")]
		public string? Priority { get; set; }

		[JsonProperty("status")]
		public string? Status { get; set; }
	}
}

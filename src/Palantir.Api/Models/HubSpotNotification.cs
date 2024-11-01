using Newtonsoft.Json;

namespace Palantir.Api.Models
{
	public class HubSpotNotification
	{
		[JsonProperty("eventId")]
		public long EventId { get; set; }

		[JsonProperty("subscriptionId")]
		public long SubscriptionId { get; set; }

		[JsonProperty("portalId")]
		public long PortalId { get; set; }

		[JsonProperty("appId")]
		public long AppId { get; set; }

		[JsonProperty("occurredAt")]
		public long OccurredAt { get; set; }

		[JsonProperty("subscriptionType")]
		public string SubscriptionType { get; set; }

		[JsonProperty("attemptNumber")]
		public int AttemptNumber { get; set; }

		[JsonProperty("objectId")]
		public long ObjectId { get; set; }

		[JsonProperty("changeFlag")]
		public string ChangeFlag { get; set; }

		[JsonProperty("changeSource")]
		public string ChangeSource { get; set; }

		//public string? EventType { get; set; }
	}
}

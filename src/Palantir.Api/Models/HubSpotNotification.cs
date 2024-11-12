using Newtonsoft.Json;

namespace Palantir.Api.Models
{
	public class HubSpotNotification
	{
		[JsonProperty("eventId")]
		public long EventId { get; set; }

		[JsonProperty("subscriptionType")]
		public string SubscriptionType { get; set; }

		[JsonProperty("objectId")]
		public long ObjectId { get; set; }

		[JsonProperty("propertyName")]
		public string? PropertyName { get; set; }

		[JsonProperty("propertyValue")]
		public string? PropertyValue { get; set; }

		[JsonProperty("changeFlag")]
		public string? ChangeFlag { get; set; }
	}
}

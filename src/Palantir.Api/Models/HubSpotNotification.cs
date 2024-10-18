namespace Palantir.Api.Models
{
	public class HubSpotNotification
	{
		public long EventId { get; set; }
		public long SubscriptionId { get; set; }
		public long PortalId { get; set; }
		public long AppId { get; set; }
		public long OccurredAt { get; set; }
		public string SubscriptionType { get; set; }
		public int AttemptNumber { get; set; }
		public long ObjectId { get; set; }
		public string ChangeFlag { get; set; }
		public string ChangeSource { get; set; }
		public string EventType { get; set; }
	}
}

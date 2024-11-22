namespace Palantir.Api.Models
{
	/// <summary>
	/// Represents a HubSpot notification response.
	/// </summary>
	public class HubSpotNotificationResponse
	{
		public string? EventId { get; set; }
		public string? Message { get; set; }
	}
}

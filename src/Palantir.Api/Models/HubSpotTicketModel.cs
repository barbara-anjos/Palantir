namespace Palantir.Api.Models
{
	/// <summary>
	/// Represents a HubSpot ticket model.
	/// </summary>
	public class HubSpotTicketModel
	{
		/// <summary>
		/// Represents a HubSpot webhook request.
		/// </summary>
		public class HubSpotWebhookRequest
		{
			/// <summary>
			/// Gets or sets the ID of the ticket in HubSpot.
			/// </summary>
			public string ObjectId { get; set; }

			/// <summary>
			/// Gets or sets the list of changed properties in HubSpot.
			/// </summary>
			public List<HubSpotChangedProperty> HubSpotChangedProperties { get; set; }

			/// <summary>
			/// Represents a HubSpot ticket response.
			/// </summary>
			public class HubSpotTicketResponse
			{
				/// <summary>
				/// Gets or sets the ID of the ticket.
				/// </summary>
				public string Id { get; set; }

				/// <summary>
				/// Gets or sets the properties of the ticket.
				/// </summary>
				public HubSpotTicketProperties Properties { get; set; }
			}

			/// <summary>
			/// Represents the properties of a HubSpot ticket.
			/// </summary>
			public class HubSpotTicketProperties
			{
				/// <summary>
				/// Gets or sets the ID of the ticket.
				/// </summary>
				public string Id { get; set; }

				/// <summary>
				/// Gets or sets the name of the ticket.
				/// </summary>
				public string Name { get; set; }

				/// <summary>
				/// Gets or sets the status of the ticket.
				/// </summary>
				public string Status { get; set; }

				/// <summary>
				/// Gets or sets the priority of the ticket.
				/// </summary>
				public string Priority { get; set; }

				/// <summary>
				/// Gets or sets the creation date of the ticket.
				/// </summary>
				public DateTime CreatedAt { get; set; }

				/// <summary>
				/// Gets or sets the send date of the ticket.
				/// </summary>
				public DateTime SendAt { get; set; }

				/// <summary>
				/// Gets or sets the pipeline of the ticket.
				/// </summary>
				public string Pipeline { get; set; }
			}

			/// <summary>
			/// Represents a changed property in HubSpot.
			/// </summary>
			public class HubSpotChangedProperty
			{
				/// <summary>
				/// Gets or sets the name of the changed property.
				/// </summary>
				public string PropertyName { get; set; }

				/// <summary>
				/// Gets or sets the new value of the changed property.
				/// </summary>
				public string NewValue { get; set; }
			}
		}
	}
}
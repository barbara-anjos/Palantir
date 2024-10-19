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
			public long ObjectId { get; set; }

			public List<HubSpotChangedProperty> HubSpotChangedProperties { get; set; }

			/// <summary>
			/// Represents a HubSpot ticket response.
			/// </summary>
			public class HubSpotTicketResponse
			{
				public string Id { get; set; }

				public HubSpotTicketProperties Properties { get; set; }
			}

			/// <summary>
			/// Represents the properties of a HubSpot ticket.
			/// </summary>
			public class HubSpotTicketProperties
			{
				public string Id { get; set; }

				public string Name { get; set; }

				public string Status { get; set; }

				/// <summary>
				/// Property 'Prioridade' in HubSpot
				/// </summary>
				public string Priority { get; set; }

				/// <summary>
				/// Property 'Prioridade - Segfy' in HubSpot
				/// </summary> 
				public string PrioritySegfy { get; set; }

				/// <summary>
				/// Gets or sets the creation date of the ticket.
				/// </summary>
				public DateTime CreatedAt { get; set; }

				/// <summary>
				/// Gets or sets the 'data_envio_dev' property of the ticket.
				/// </summary>
				public DateTime? SendAt { get; set; }

				public string Pipeline { get; set; }

				public string LinkIntranet { get; set; }

				public string Category { get; set; }

				public string Services { get; set; }

				/// <summary>
				/// Property 'Descrição do ticket' in HubSpot
				/// </summary>
				public string Content { get; set; }
			}

			/// <summary>
			/// Represents a changed property in HubSpot.
			/// </summary>
			public class HubSpotChangedProperty
			{
				public string PropertyName { get; set; }

				public string NewValue { get; set; }
			}

			/// <summary>
			/// Represents the notes of a HubSpot ticket.
			/// </summary>
			public class HubSpotTicketNotes
			{
				public long EngagementId { get; set; }
				public string Type { get; set; }
				public DateTime CreatedAt { get; set; }
				public DateTime UpdatedAt { get; set; }

				public string Body { get; set; }

				public List<HubSpotTicketNotesAttachment> NotesAttachment { get; set; }

				public HubSpotTicketNotes()
				{
					NotesAttachment = new List<HubSpotTicketNotesAttachment>();
				}

				/// <summary>
				/// Represents an attachment of a HubSpot ticket note.
				/// </summary>
				public class HubSpotTicketNotesAttachment
				{
					public long FileId { get; set; }
					public string FileName { get; set; }
					public string FileUrl { get; set; }
				}
			}
		}
	}
}
using Newtonsoft.Json;
using System.Text.Json.Serialization;

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
				[JsonProperty("id")]
				public string Id { get; set; }

				[JsonProperty("properties")]
				public HubSpotTicketProperties Properties { get; set; }

				[JsonProperty("createdAt")]
				public DateTime CreatedAt { get; set; }

				[JsonProperty("updatedAt")]
				public DateTime UpdatedAt { get; set; }

				[JsonProperty("archived")]
				public bool Archived { get; set; }
			}

			/// <summary>
			/// Represents the properties of a HubSpot ticket.
			/// </summary>
			public class HubSpotTicketProperties
			{
				[JsonProperty("hs_object_id")]
				public string Id { get; set; }

				[JsonProperty("subject")]
				public string Name { get; set; }

				[JsonProperty("hs_pipeline_stage")]
				public string Status { get; set; }

				/// <summary>
				/// Property 'Prioridade' in HubSpot
				/// </summary>
				[JsonProperty("hs_ticket_priority")]
				public string Priority { get; set; }

				/// <summary>
				/// Property 'Prioridade - Segfy' in HubSpot
				/// </summary> 
				[JsonProperty("prioridade")]
				public string PrioritySegfy { get; set; }

				/// <summary>
				/// Gets or sets the creation date of the ticket.
				/// </summary>
				[JsonProperty("createdate")]
				public DateTime CreateDate { get; set; }

				/// <summary>
				/// Gets or sets the 'data_envio_dev' property of the ticket.
				/// </summary>
				[JsonProperty("data_envio___dev")]
				public DateTime? SendAt { get; set; }

				[JsonProperty("hs_lastmodifieddate")]
				public DateTime? LastModifiedDate { get; set; }

				[JsonProperty("hs_pipeline")]
				public string Pipeline { get; set; }

				[JsonProperty("link_intranet")]
				public string LinkIntranet { get; set; }

				[JsonProperty("categoria")]
				public string Category { get; set; }

				[JsonProperty("servicos__clonado_")]
				public string Services { get; set; }

				/// <summary>
				/// Property 'Descrição do ticket' in HubSpot
				/// </summary>
				[JsonProperty("content")]
				public string? Content { get; set; }
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
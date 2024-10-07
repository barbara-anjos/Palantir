namespace Palantir.Api.Models
{
    public class HubSpotTicketModel
    {
        public class HubSpotWebhookRequest
        {
            public string ObjectId { get; set; } // ID do tíquete no HubSpot
            public List<HubSpotChangedProperty> HubSpotChangedProperties { get; set; }

            public class HubSpotTicketResponse
            {
                public string Id { get; set; }
                public HubSpotTicketProperties Properties { get; set; }
            }

            public class HubSpotTicketProperties
            {
                public string Id { get; set; }
                public string Name { get; set; }
                public string Status { get; set; }
                public string Priority { get; set; }
                public DateTime CreatedAt { get; set; }
                public DateTime SendAt { get; set; }
                public string Pipeline { get; set; }
            }

            public class HubSpotChangedProperty
            {
                public string PropertyName { get; set; }
                public string NewValue { get; set; }
            }
        }
    }
}
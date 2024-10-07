namespace Palantir.Api.Models
{
    public class ClickUpTaskModel
    {
        public class ClickUpWebhookRequest
        {
            public string Event { get; set; }
            public ClickUpTask Task { get; set; }
            public string WorkspaceId { get; set; }
            public string TeamId { get; set; }
            public DateTime Timestamp { get; set; }
            public string ObjectId => Task?.Id; // Acessa a ID da tarefa
        }

        public class ClickUpTask
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public List<ClickUpCustomField> CustomFields { get; set; }
        }

        public class ClickUpCustomField
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class ClickUpTaskResponse
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public List<ClickUpCustomField> CustomFields { get; set; }
            public string DueDate { get; set; }
            public string Assignee { get; set; }
        }

    }
}

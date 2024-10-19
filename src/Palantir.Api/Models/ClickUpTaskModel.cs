using static Palantir.Api.Models.ClickUpTaskModel;

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
			public string Description { get; set; }
            public long StartDate { get; set; }
			public long DueDate { get; set; }
            public long TimeEstimate { get; set; }
            public int Priority { get; set; }
            public List<Tags> Tags { get; set; }
			public List<ClickUpCustomField> CustomFields { get; set; }
        }

		public class Tags
		{
			public string Name { get; set; }
		}


		public class ClickUpCustomField
        {
            public string Id { get; set; }
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

		public class ClickUpTaskUpdateData
		{
			public string Status { get; set; }
			public string Priority { get; set; }
			public DateTime DueDate { get; set; }
		}

		public class TaskList
		{
			public List<ClickUpTask> Tasks { get; set; }
		}

	}
}

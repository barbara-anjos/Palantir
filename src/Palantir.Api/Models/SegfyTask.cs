namespace Palantir.Api.Models
{
	/// <summary>
	/// Represents a generic task
	/// </summary>
	public class SegfyTask
	{
		public string TicketId { get; set; }
		public string TaskId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public long StartDate { get; set; }
		public long DueDate { get; set; }
		public int TimeEstimate { get; set; }
		public int PriorityId { get; set; }
		public string PriorityName { get; set; }
		public List<string> Tags { get; set; }
		public List<string> Comments { get; set; }
		public string Services { get; set; }
		public string Category { get; set; }
		public string LinkIntranet { get; set; }
		public string Pipeline { get; set; }
		public List<string> CustomFields { get; set; }
		public string Status { get; set; }
	}

    public class SegfyTaskList
    {
        public List<SegfyTask> Tasks { get; set; }
    }
}

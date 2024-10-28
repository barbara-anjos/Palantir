namespace Palantir.Api.Models
{
	/// <summary>
	/// 
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
		public int Priority { get; set; }
		public List<string> Tags { get; set; }
		public List<string> Comments { get; set; }
		public string Services { get; set; }
		public string Category { get; set; }
		public string LinkIntranet { get; set; }
		public string Pipeline { get; set; }
	}

    public class SegfyTaskList
    {
        public List<SegfyTask> Tasks { get; set; }
    }
}

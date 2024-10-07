namespace Palantir.Api.Models
{
    public class ClickUpTaskUpdateModel
    {
        public class ClickUpTaskUpdateData
        {
            public string Status { get; set; }
            public string Priority { get; set; }
            public DateTime DueDate { get; set; }
        }

    }
}

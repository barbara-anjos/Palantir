namespace Palantir.Api.Enums
{
	/// <summary>
	/// Priority ticket SLA in hours
	/// </summary>
	public enum HubSpotTicketSLA
	{
		LOW = 160,
		MEDIUM = 80,
		HIGH = 24,
		URGENT = 8
	}

	public class HubSpotTicketPrioritySLAConstants
	{
		public static readonly Dictionary<string, HubSpotTicketSLA> PriorityDictionary = new Dictionary<string, HubSpotTicketSLA>()
		{
			{ "LOW", HubSpotTicketSLA.LOW },
			{ "MEDIUM", HubSpotTicketSLA.MEDIUM },
			{ "HIGH", HubSpotTicketSLA.HIGH },
			{ "URGENT", HubSpotTicketSLA.URGENT },
		};

		public static readonly Dictionary<HubSpotTicketSLA, int> PriorityMap = new Dictionary<HubSpotTicketSLA, int>()
		{
			{ HubSpotTicketSLA.LOW, 4 },
			{ HubSpotTicketSLA.MEDIUM, 3 },
			{ HubSpotTicketSLA.HIGH, 2 },
			{ HubSpotTicketSLA.URGENT, 1 },
		};

		public static HubSpotTicketSLA GetPriority(string priority)
		{
			switch (priority.ToUpper())
			{
				case "BAIXA":
				case "BAIXO":
				case "LOW":
					return HubSpotTicketSLA.LOW;

				case "MÉDIA":
				case "MÉDIO":
				case "NORMAL":
				case "MEDIUM":
					return HubSpotTicketSLA.MEDIUM;

				case "ALTA":
				case "ALTO":
				case "HIGH":
					return HubSpotTicketSLA.HIGH;

				case "URGENTE":
				case "URGENT":
					return HubSpotTicketSLA.URGENT;

				default:
					return HubSpotTicketSLA.MEDIUM;
			}
		}
	}
}

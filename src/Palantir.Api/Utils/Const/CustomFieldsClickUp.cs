namespace Palantir.Api.Utils.Const
{
	/// <summary>
	/// The ids custom fields in ClickUp. The id name matches the custom field name in ClickUp
	/// </summary>
	public static class CustomFieldsClickUp
	{
		public const string ticketId = "471039af-a966-4bb0-aab3-5fd1cb92f014";
		public const string linkHubSpot = "4b809840-77df-4f7d-8e9a-8bef1000830e";
		public const string urlIntranet = "23a17861-33b0-476a-8ba0-981f3430ea3a";
		public const string tipo = "81250100-87b4-4174-8aad-0e699b856600";
		public const string funcionalidade = "3a672cc9-e65d-4162-8e30-9db1c52d5763";


		/// <summary>
		/// The ids of values for custom field 'Tipo' in ClickUp
		/// </summary>
		public static readonly Dictionary<string, List<string>> TipoValues = new Dictionary<string, List<string>>
		{
			{ "BUG", new List<string> { "9cd5709d-acdd-4eb2-a884-7f7200622664" } },
			{ "AJUSTE", new List<string> { "bf52ac14-42ce-4105-8551-5863dab5adc9" } },
			{ "Não Erro", new List<string> { "7f32912b-0b0d-4a50-9969-5ab45b82167f" } },
			{ "Indevido", new List<string> { "00539e25-1ffa-416a-a021-62af226b9e71" } },
			{ "Solicitação", new List<string> { "05154e22-d405-441c-a9f1-e158a10be81a" } },
			{ "Mudança regra", new List<string> { "c5c438a6-3528-4024-a800-e9238e07ac5b" } }
		};

		/// <summary>
		/// The ids of values for custom field 'Funcionalidade' in ClickUp
		/// </summary>
		public static readonly Dictionary<string, List<string>> FuncionalidadeValues = new Dictionary<string, List<string>>
		{
			{ "ServiceA", new List<string> { "ServiceALabelId" } },
			{ "ServiceB", new List<string> { "ServiceBLabelId" } }
		};
	}
}

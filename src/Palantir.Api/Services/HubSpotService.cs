using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Palantir.Api.Models.HubSpotTicketModel;
using System.Net.Http.Headers;
using System.Text;
using Palantir.Api.Configurations;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;
using Palantir.Api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

public class HubSpotService : ICustomerTicketService<HubSpotTicketResponse>
{
	private readonly HttpClient _httpClient;
	private readonly string _apiKey;
	private readonly string _baseUrl;
	private readonly string _propertiesUrl;

	public HubSpotService(HttpClient httpClient, IOptions<HubSpotSettings> hubSpotSettings)
	{
		_httpClient = httpClient;
		_apiKey = hubSpotSettings.Value.ApiKey;
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
		_baseUrl = hubSpotSettings.Value.BaseUrl;
		_propertiesUrl = hubSpotSettings.Value.PropertiesUrl;
	}

	/// <summary>
	/// Get a ticket from HubSpot by ID and return the HubSpotTicket model
	/// </summary>
	/// <param name="ticketId"></param>
	/// <returns></returns>
	public async Task<HubSpotTicketResponse> GetTicketByIdAsync(long ticketId)
	{
		var requestUrl = $"{_baseUrl}/{ticketId}/{_propertiesUrl}";

		var response = await _httpClient.GetAsync(requestUrl);

		if (response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync();
			var ticketData = JsonConvert.DeserializeObject<HubSpotTicketResponse>(content);

			return ticketData;
		}

		return null;
	}

	//// Método para atualizar o status de um tíquete no HubSpot
	//public async Task UpdateTicketStatusAsync(string ticketId, string newStatus)
	//{
	//    var requestUrl = $"{_baseUrl}/{ticketId}";
	//    var data = new
	//    {
	//        properties = new
	//        {
	//            status = newStatus
	//        }
	//    };

	//    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
	//    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

	//    var response = await _httpClient.PatchAsync(requestUrl, content);

	//    if (!response.IsSuccessStatusCode)
	//    {
	//        throw new Exception("Erro ao atualizar o status do tíquete no HubSpot.");
	//    }
	//}
}


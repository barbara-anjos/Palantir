using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Palantir.Api.Models.HubSpotTicketModel;
using System.Net.Http.Headers;
using System.Text;
using Palantir.Api.Configurations;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;
using Palantir.Api.Interfaces;

public class HubSpotService : ICustomerTicketService<HubSpotTicketProperties>
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _propertiesUrl;

	public HubSpotService(HttpClient httpClient, IOptions<HubSpotSettings> hubSpotSettings)
    {
        _httpClient = httpClient;
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
		_apiKey = hubSpotSettings.Value.ApiKey;
		_baseUrl = hubSpotSettings.Value.BaseUrl;
		_propertiesUrl = hubSpotSettings.Value.PropertiesUrl;
	}

	/// <summary>
	/// Get a ticket from HubSpot by ID and return the HubSpotTicket model
	/// </summary>
	/// <param name="ticketId"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<HubSpotTicketProperties> GetTicketByIdAsync(long ticketId)
    {
        var requestUrl = $"{_baseUrl}/{ticketId}/{_propertiesUrl}";       

        var response = await _httpClient.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var ticketData = JsonConvert.DeserializeObject<HubSpotTicketResponse>(content);

            //Maps the ticket data to the HubSpotTicketProperties model
            return new HubSpotTicketProperties
            {
                Id = ticketData.Id,
                Name = ticketData.Properties.Name,
                Status = ticketData.Properties.Status,
                Priority = ticketData.Properties.Priority,
                CreatedAt = ticketData.Properties.CreatedAt,
                SendAt = ticketData.Properties.SendAt,
                Services = ticketData.Properties.Services,
                Pipeline = ticketData.Properties.Pipeline,
                LinkIntranet = ticketData.Properties.LinkIntranet,
                PrioritySegfy = ticketData.Properties.PrioritySegfy,
                Category = ticketData.Properties.Category,
                Content = ticketData.Properties.Content
            };
        }

        throw new Exception("Error searching ticket in HubSpot.");
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


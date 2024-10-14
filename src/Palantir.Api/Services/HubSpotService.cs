using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Palantir.Api.Models.HubSpotTicketModel;
using System.Net.Http.Headers;
using System.Text;
using Palantir.Api.Configurations;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

public class HubSpotService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public HubSpotService(HttpClient httpClient, IOptions<HubSpotSettings> hubSpotSettings)
    {
        _httpClient = httpClient;
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
		_apiKey = hubSpotSettings.Value.ApiKey;
    }

    // Método para buscar um tíquete no HubSpot por ID e retornar o modelo HubSpotTicket
    public async Task<HubSpotTicketProperties> GetTicketByIdAsync(string ticketId)
    {
        var requestUrl = $"https://api.hubapi.com/crm/v3/objects/tickets/{ticketId}";       

        var response = await _httpClient.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var ticketData = JsonConvert.DeserializeObject<HubSpotTicketResponse>(content);

            // Mapeia a resposta da API para o modelo HubSpotTicket
            return new HubSpotTicketProperties
            {
                Id = ticketData.Id,
                Name = ticketData.Properties.Name,
                Status = ticketData.Properties.Status,
                Priority = ticketData.Properties.Priority,
                CreatedAt = ticketData.Properties.CreatedAt,
                SendAt = ticketData.Properties.SendAt
            };
        }

        throw new Exception("Erro ao buscar o tíquete no HubSpot.");
    }

    //// Método para atualizar o status de um tíquete no HubSpot
    //public async Task UpdateTicketStatusAsync(string ticketId, string newStatus)
    //{
    //    var requestUrl = $"https://api.hubapi.com/crm/v3/objects/tickets/{ticketId}";
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


using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Palantir.Api.Models.HubSpotTicketModel;
using System.Net.Http.Headers;
using System.Text;
using Palantir.Api.Configurations;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;
using Palantir.Api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Palantir.Api.Models;
using Flurl.Http;
using static Palantir.Api.Models.ClickUpTaskModel;
using System.Threading.Tasks;
using Palantir.Api.Mappers;

public class HubSpotService : ICustomerTicketService<HubSpotTicketResponse>
{
	private readonly HttpClient _httpClient;
	private readonly string _apiKey;
	private readonly string _baseUrl;
	private readonly string _propertiesUrl;
	private readonly StatusMapper _statusMapper;

	public HubSpotService(HttpClient httpClient, IOptions<HubSpotSettings> hubSpotSettings, StatusMapper statusMapper)
	{
		_httpClient = httpClient;
		_apiKey = hubSpotSettings.Value.ApiKey;
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
		_baseUrl = hubSpotSettings.Value.BaseUrl;
		_propertiesUrl = hubSpotSettings.Value.PropertiesUrl;
		_statusMapper = statusMapper;
	}

	/// <summary>
	/// Get a ticket from HubSpot by ID and return the HubSpotTicket model
	/// </summary>
	/// <param name="ticketId"></param>
	/// <returns></returns>
	public async Task<HubSpotTicketResponse> GetTicketByIdAsync(long ticketId)
	{
		try
		{
			var requestUrl = $"{_baseUrl}/{ticketId}/{_propertiesUrl}";

			var request = requestUrl.WithOAuthBearerToken(_apiKey);

			var response = await request.GetAsync();
			var jsonResponse = await response.ResponseMessage.Content.ReadAsStringAsync();

			if (response.ResponseMessage.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<HubSpotTicketResponse>(jsonResponse);
			}
			else
			{
				throw new Exception($"Failed to get ticket from HubSpot. {jsonResponse}");
			}
		}
		catch (FlurlHttpException ex)
		{
			var errorContent = await ex.GetResponseStringAsync();
			throw new Exception($"Failed to get ticket from HubSpot. {errorContent}");
		}
	}

	/// <summary>
	/// Update a ticket in HubSpot
	/// </summary>
	/// <param name="ticket"></param>
	/// <param name="ticketId"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<HubSpotTicketResponse> UpdateTicket(HubSpotTicketResponse ticket, string ticketId)
	{
		try
		{
			var requestUrl = $"{_baseUrl}/{ticketId}";
			var request = requestUrl.WithOAuthBearerToken(_apiKey);

			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			string json = JsonConvert.SerializeObject(ticket, settings);
			var postData = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await request.PatchAsync(postData);

			var jsonResponse = await response.ResponseMessage.Content.ReadAsStringAsync();

			if (response.ResponseMessage.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<HubSpotTicketResponse>(jsonResponse);
			}
			else
			{
				throw new Exception($"Failed to update ticket in HubSpot. {jsonResponse}");
			}
		}
		catch (FlurlHttpException ex)
		{
			var errorContent = await ex.GetResponseStringAsync();
			throw new Exception($"Failed to update ticket in HubSpot. {errorContent}");
		}
	}

	/// <summary>
	/// Update a ticket in HubSpot from a task in ClickUp
	/// </summary>
	/// <param name="ticketId"></param>
	/// <param name="updatedData"></param>
	/// <param name="pipeline"></param>
	/// <returns></returns>
	public async Task<bool> UpdateTicketFromTask(string ticketId, SegfyTask updatedData, string pipeline)
	{
		var data = new HubSpotTicketResponse
		{
			Properties =
			{
				Priority = updatedData.PriorityName,
				Status = _statusMapper.GetHubSpotStatus(updatedData.Status, pipeline)
			}
		};

		var updatedTask = await UpdateTicket(data, ticketId);
		return updatedTask != null;
	}
}


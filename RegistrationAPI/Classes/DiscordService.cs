using System.Text;
using System.Text.Json;

namespace RegistrationAPI.Classes;
public class DiscordService
{
	private readonly HttpClient _httpClient;

	public DiscordService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task SendDiscordMessageAsync(string webhookUrl, string message)
	{
		var payload = new
		{
			content = message
		};

		var json = JsonSerializer.Serialize(payload);

		var response = await _httpClient.PostAsync(
			webhookUrl,
			new StringContent(json, Encoding.UTF8, "application/json")
		);

		response.EnsureSuccessStatusCode();
	}
}



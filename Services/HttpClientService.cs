using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using EarthquakeReporter.Configuration;

namespace EarthquakeReporter.Services
{
    public class HttpClientService<T> : IHttpClientService<T> where T : new()
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly ILogger<HttpClientService<T>> _logger;

        public HttpClientService(HttpClient httpClient, IOptions<AppSettings> appSettings, ILogger<HttpClientService<T>> logger)
        {
            _httpClient = httpClient;
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T> GetDataAsync(string resource, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync(resource, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API request failed with status code {response.StatusCode}. Response: {errorContent}");
                    response.EnsureSuccessStatusCode();
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var data = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (data == null)
                {
                    throw new Exception("Failed to deserialize response content.");
                }

                return data;
            }
            catch (HttpRequestException httpRequestException)
            {
                _logger.LogError(httpRequestException, "An error occurred while sending the HTTP request.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                throw;
            }
        }
    }
}

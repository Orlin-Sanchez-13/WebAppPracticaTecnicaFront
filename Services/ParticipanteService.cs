using Microsoft.Extensions.Options;
using System.Text.Json;
using WebAppPractica.Models;

namespace WebAppPractica.Services;

public class ParticipanteService : IParticipanteService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly PathsConfig _pathsConfig;
    private readonly ILogger<ParticipanteService> _logger;

    public ParticipanteService(
        HttpClient httpClient,
        IOptions<ApiSettings> apiSettings,
        IOptions<PathsConfig> pathsConfig,
        ILogger<ParticipanteService> logger)
    {
        _httpClient = httpClient;
        _apiSettings = apiSettings.Value;
        _pathsConfig = pathsConfig.Value;
        _logger = logger;
        
        _logger.LogInformation("ParticipanteService inicializado");
        _logger.LogInformation("ApiBackend: {ApiBackend}", _apiSettings.ApiBackend);
        _logger.LogInformation("PathPrincipal: {PathPrincipal}", _pathsConfig.PathPrincipal);
        _logger.LogInformation("ListadoParticipantes: {ListadoParticipantes}", _pathsConfig.ListadoParticipantes);
    }

    public async Task<IEnumerable<ParticipanteDto>> ObtenerParticipantesAsync()
    {
        try
        {
            var endpoint = $"{_apiSettings.ApiBackend}{_pathsConfig.PathPrincipal}{_pathsConfig.ListadoParticipantes}";
            var response = await _httpClient.GetAsync(endpoint);
            
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var participantes = JsonSerializer.Deserialize<IEnumerable<ParticipanteDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var lista = participantes?.ToList() ?? new List<ParticipanteDto>();
            _logger.LogInformation("Deserialización exitosa. Total registros: {Count}", lista.Count);

            return lista;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error HTTP al consumir el API de participantes. Mensaje: {Message}", ex.Message);
            return Enumerable.Empty<ParticipanteDto>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error JSON al deserializar la respuesta del API. Mensaje: {Message}", ex.Message);
            return Enumerable.Empty<ParticipanteDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener participantes. Tipo: {Type}, Mensaje: {Message}", ex.GetType().Name, ex.Message);
            return Enumerable.Empty<ParticipanteDto>();
        }
    }
}
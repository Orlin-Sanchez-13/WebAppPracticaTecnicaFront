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

    public async Task<RegistroParticipanteResultado> RegistrarParticipanteAsync(ParticipanteDto participante)
    {
        try
        {
            var endpoint = $"{_apiSettings.ApiBackend}{_pathsConfig.PathPrincipal}{_pathsConfig.RegistroParticipantes}";
            var content = new StringContent(JsonSerializer.Serialize(participante), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Participante registrado correctamente.");
                return new RegistroParticipanteResultado
                {
                    Success = true,
                    Message = "Participante registrado correctamente.",
                    Data = participante
                };
            }

            var fieldErrors = TryParseFieldErrors(body);
            var generalError = TryParseGeneralError(body);

            _logger.LogWarning("Error al registrar participante. Respuesta: {Body}", body);

            return new RegistroParticipanteResultado
            {
                Success = false,
                Message = "No se pudo registrar el participante.",
                Data = participante,
                FieldErrors = fieldErrors,
                GeneralError = generalError,
                RawErrorJson = body
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error HTTP al registrar participante. Mensaje: {Message}", ex.Message);
            return new RegistroParticipanteResultado
            {
                Success = false,
                Message = "Error de comunicación con el servicio.",
                Data = participante,
                GeneralError = ex.Message
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error JSON al serializar el participante. Mensaje: {Message}", ex.Message);
            return new RegistroParticipanteResultado
            {
                Success = false,
                Message = "Error al procesar los datos enviados.",
                Data = participante,
                GeneralError = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al registrar participante. Tipo: {Type}, Mensaje: {Message}", ex.GetType().Name, ex.Message);
            return new RegistroParticipanteResultado
            {
                Success = false,
                Message = "Error inesperado al registrar.",
                Data = participante,
                GeneralError = ex.Message
            };
        }
    }

    private static Dictionary<string, string[]>? TryParseFieldErrors(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string[]>>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }

    private static string? TryParseGeneralError(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.ValueKind == JsonValueKind.String)
            {
                return doc.RootElement.GetString();
            }

            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("message", out var messageElement) &&
                messageElement.ValueKind == JsonValueKind.String)
            {
                return messageElement.GetString();
            }
        }
        catch
        {
            return body;
        }

        return body;
    }
}
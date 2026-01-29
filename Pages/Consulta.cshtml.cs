using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebAppPractica.Models;
using WebAppPractica.Services;

namespace WebAppPractica.Pages
{
    public class Index1Model : PageModel
    {
        private readonly IParticipanteService _participanteService;
        private readonly ILogger<Index1Model> _logger;

        public string ParticipantesJson { get; set; } = "[]";
        public string ErrorMessage { get; set; } = string.Empty;

        public Index1Model(IParticipanteService participanteService, ILogger<Index1Model> logger)
        {
            _participanteService = participanteService;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            try
            {
                var participantes = await _participanteService.ObtenerParticipantesAsync();
                
                var listaParticipantes = participantes.ToList();
                ParticipantesJson = JsonSerializer.Serialize(listaParticipantes, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar participantes");
                ErrorMessage = ex.Message;
                ParticipantesJson = "[]";
            }
        }
    }
}

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppPractica.Models;
using WebAppPractica.Services;

namespace WebAppPractica.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly IParticipanteService _participanteService;
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(IParticipanteService participanteService, ILogger<PrivacyModel> logger)
        {
            _participanteService = participanteService;
            _logger = logger;
        }

        [BindProperty]
        public ParticipanteDto Participante { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _participanteService.RegistrarParticipanteAsync(Participante);

            TempData["ResultStatus"] = result.Success ? "success" : "error";
            TempData["ResultData"] = JsonSerializer.Serialize(result.Data);

            if (result.Success)
            {
                TempData["ResultMessage"] = result.Message ?? "Datos Guardados correctamente.";
            }
            else
            {
                if (result.FieldErrors?.Count > 0)
                {
                    TempData["ResultErrors"] = JsonSerializer.Serialize(result.FieldErrors);
                    TempData["ResultMessage"] = "ERROR - Revisa los campos marcados.";
                }
                else
                {
                    TempData["ResultMessage"] = result.GeneralError ?? result.Message ?? "No se pudo registrar el participante.";
                }
            }

            return RedirectToPage();
        }
    }
}

using WebAppPractica.Models;

namespace WebAppPractica.Services;

public interface IParticipanteService
{
    Task<IEnumerable<ParticipanteDto>> ObtenerParticipantesAsync();
    Task<RegistroParticipanteResultado> RegistrarParticipanteAsync(ParticipanteDto participante);
}
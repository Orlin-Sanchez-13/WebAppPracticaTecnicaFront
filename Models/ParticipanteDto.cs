using System.ComponentModel.DataAnnotations;

namespace WebAppPractica.Models;

public class ParticipanteDto
{
    public int IdUsuario { get; set; }
    public string NombreCompania { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string NombreContacto { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
}
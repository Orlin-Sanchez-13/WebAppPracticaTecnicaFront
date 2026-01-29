namespace WebAppPractica.Models;

public class RegistroParticipanteResultado
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ParticipanteDto Data { get; set; } = new();
    public Dictionary<string, string[]>? FieldErrors { get; set; }
    public string? GeneralError { get; set; }
    public string? RawErrorJson { get; set; }
}
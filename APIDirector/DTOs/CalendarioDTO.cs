namespace APIDirector.DTOs
{
    public class CalendarioDTO
    {
        public int IdCalendario { get; set; }

        public string Titulo { get; set; } = null!;

        public DateOnly Fecha { get; set; }

        public string? Descripcion { get; set; }
    }
}

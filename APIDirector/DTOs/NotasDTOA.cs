namespace APIDirector.DTOs
{
    public class NotasDTOA
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }

        public int IdAlumno { get; set; }
        public string NombreAlumno { get;set; } = null!;
        public string Grado { get; set; } = "";
        public string Seccion { get; set; } = "";
    }
}

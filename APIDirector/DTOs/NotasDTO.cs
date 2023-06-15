namespace APIDirector.DTOs
{
    public class NotasDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }
        
        public int IdAlumno { get; set; }

      
    }
}

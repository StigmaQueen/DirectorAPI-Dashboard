namespace APIDirector.DTOs
{
    public class DirectorDTO
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
    }
}

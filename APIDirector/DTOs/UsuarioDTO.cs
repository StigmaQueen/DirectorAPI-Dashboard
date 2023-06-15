namespace APIDirector.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
        public int Rol { get; set; }
    }
}

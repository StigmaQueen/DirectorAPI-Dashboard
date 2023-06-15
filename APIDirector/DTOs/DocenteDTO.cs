namespace APIDirector.DTOs
{
    public enum TipoDocente
    {
        Especial = 1,
        Grupo = 2
    }
    public class DocenteDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string ApellidoPaterno { get; set; } = null!;

        public string ApellidoMaterno { get; set; } = null!;

        public string Correo { get; set; } = null!;

        public string Telefono { get; set; } = null!;

        public int Edad { get; set; }

        public TipoDocente TipoDocente { get; set; }

        public string Usuario1 { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
        public int Rol { get; set; }

        public int IdUsuario { get; set; } = 0;
        public int IdGrupo { get; set; } = 0;
        public int IdAsignatura { get; set; } = 0;
    }
}

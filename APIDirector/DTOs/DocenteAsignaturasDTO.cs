namespace APIDirector.DTOs
{
    public class DocenteAsignaturasDTO
    {
        public int IdDocente { get; set; }
        public string NombreProfe { get; set; } = "";
        public string PrimerApellido { get; set; } = "";
        public string SegundoApellido { get; set; } = "";
        public int IdAsignatura { get; set; }
        public string asignatura { get; set; } = "";
    }
}

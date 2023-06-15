namespace APIDirector.DTOs
{
    public class DocenteGrupocs
    {
        public int Id { get; set; }
        public string NombreProfe { get; set; } = "";
        public string PrimerApellido { get; set; } = "";
        public string SegundoApellido { get; set; } = "";

        public TipoDocente tipoDocente { get; set; }
        public string Grado { get; set; } = "";
        public string Seccion { get; set; } = "";

        public short periodo { get; set; }
        public int IdDocente { get; set; }

        public int IdGrupo { get; set; }

        public int IdPeriodo { get; set; }
    }
}

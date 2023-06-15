using APIDirector.DTOs;
using APIDirector.Models;
using APIDirector.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;

namespace APIDirector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorController : ControllerBase
    {
        Repository<Usuario> UsuarioRepo;
        Repository<Director> DirectorRepo;
        Repository<Docente> DocenteRepo;
        Repository<DocenteAsignatura> DocenteAsignaturaRepo;
        Repository<DocenteGrupo> DocenteGrupoRepo;
        Repository<Asignatura> AsigRepo;
        Repository<Grupo> GrupoRepo;
        Repository<Periodo> PeriodoRepo;
        Repository<Notasdirector> NotasRepo;
        Repository<Calendario> CalendarioRepo;
        Repository<Alumno> alumnoRepo;


        public DirectorController(Sistem21PrimariaContext context)
        {
            UsuarioRepo = new(context);
            DirectorRepo = new(context);
            DocenteRepo = new(context);
            DocenteAsignaturaRepo = new(context);
            DocenteGrupoRepo = new(context);
            AsigRepo = new(context);
            PeriodoRepo = new(context);
            GrupoRepo = new(context);
            NotasRepo = new(context);
            CalendarioRepo = new(context);
            alumnoRepo = new(context);

        }

        [HttpPost("Login")]
        public IActionResult Login(Usuario usuario)
        {
            var usuarioDirector = UsuarioRepo.Get().FirstOrDefault(x => x.Usuario1 == usuario.Usuario1 && x.Contraseña == usuario.Contraseña);
            if (usuarioDirector == null || usuarioDirector.Rol != 1)
            {
                return NotFound("Usuario o Contraseña Incorrectos");
            }

            //////
            var d = DirectorRepo.Get(usuarioDirector.Id);
            if (d != null)
            {
                Director director;
                director = new Director
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    Telefono = d.Telefono,
                    Direccion = d.Direccion,
                    Idusuario = d.Idusuario
                };
                return Ok(director);
            }

            return NotFound("Usuario o Contraseña Incorrectos");

        }
        [HttpPut("EditarMiPerfil")]
        public IActionResult EditarDatosDirector(DirectorDTO d)
        {
            var director = DirectorRepo.Get(d.Id);
            var user = UsuarioRepo.Get(d.Id);
            if (director == null) return NotFound();
            if (string.IsNullOrWhiteSpace(d.Nombre))
            {
                return BadRequest("El nombre no puede ir vacio");
            }
            if (string.IsNullOrWhiteSpace(d.Telefono))
            {
                return BadRequest("El telefono no pude ir vacio");
            }
            if (string.IsNullOrWhiteSpace(d.Direccion))
            {
                return BadRequest("La direccion no puede ir vacia");
            }
            if (string.IsNullOrWhiteSpace(d.Usuario))
            {
                return BadRequest("El nombre de usuario no puede ir vacio");
            }
            if (UsuarioRepo.Get().Any(x => x.Usuario1 == d.Usuario && x.Id != d.IdUsuario))
            {
                return BadRequest("Este nombre de usuario no está disponible");
            }
            if (string.IsNullOrWhiteSpace(d.Contraseña))
            {
                return BadRequest("La contraseña no puede ir vacia");
            }
            if (user != null)
            {
                director.Nombre = d.Nombre;
                director.Telefono = d.Telefono;
                director.Direccion = d.Direccion;
                user.Usuario1 = d.Usuario;
                user.Contraseña = d.Contraseña;
                UsuarioRepo.Update(user);
                DirectorRepo.Update(director);
            }

            return Ok();

        }

        [HttpGet("Docentes")]
        public IActionResult Get()
        {
            var docentes = DocenteRepo.Get().OrderBy(x => x.Id).ToList();
            List<DocenteDTO> lis = new List<DocenteDTO>();

            foreach (var item in docentes)
            {
                var usuario = UsuarioRepo.Get().FirstOrDefault(x => x.Id == item.IdUsuario);

                if (usuario != null)
                {
                    DocenteDTO docente = new DocenteDTO()
                    {

                        Id = item.Id,
                        ApellidoMaterno = item.ApellidoMaterno,
                        ApellidoPaterno = item.ApellidoPaterno,
                        Edad = item.Edad,
                        Correo = item.Correo,
                        Nombre = item.Nombre,
                        Telefono = item.Telefono,
                        TipoDocente = (TipoDocente)item.TipoDocente,
                        Usuario1 = usuario.Usuario1,
                        Contraseña = usuario.Contraseña,
                        IdUsuario = usuario.Id

                    };
                    lis.Add(docente);
                }

            }


            return Ok(lis);
        }
        [HttpGet("DatosDirector")]
        public IActionResult GetD()
        {
            var director = DirectorRepo.Get().OrderBy(x => x.Id).ToList();
            List<DirectorDTO> lis = new List<DirectorDTO>();
            foreach (var item in director)
            {
                var usuario = UsuarioRepo.Get().FirstOrDefault(x => x.Id == item.Idusuario);
                if (usuario != null)
                {
                    DirectorDTO director1 = new DirectorDTO()
                    {
                        Id = item.Id,
                        IdUsuario = usuario.Id,
                        Nombre = item.Nombre,
                        Telefono = item.Telefono,
                        Direccion = item.Direccion,
                        Usuario = usuario.Usuario1,
                        Contraseña = usuario.Contraseña


                    };
                    lis.Add(director1);
                }

            }
            var director2 = lis.FirstOrDefault();

            return Ok(director2);
        }
        [HttpGet("Docente")]
        public IActionResult Get(Docente d)
        {
            var docente = DocenteRepo.Get(d.Id);
            return Ok(docente);
        }

        [HttpPost("AgregarDocente")]
        public IActionResult InsertDocentes(DocenteDTO d)
        {
            if (string.IsNullOrEmpty(d.Nombre))
            {
                return BadRequest("El nombre no puede ir vacio");

            }
            if (string.IsNullOrEmpty(d.ApellidoMaterno))
            {
                return BadRequest("Ingrese el pimer apellido");

            }
            if (string.IsNullOrEmpty(d.ApellidoMaterno))
            {
                BadRequest("Ingrese el segundo apellido");
            }
            if (string.IsNullOrEmpty(d.Telefono))
            {
                return BadRequest("Ingrese un numero telefonico");

            }
            if (string.IsNullOrEmpty(d.Correo))
            {
                return BadRequest("Ingrese un correo electronico");
            }
            if (d.Edad < 18)
            {
                return BadRequest("EL docente en cuestion debe ser mayor de edad");

            }
            if (string.IsNullOrEmpty(d.Usuario1))
            {
                return BadRequest("Ingrese un nombre de usuario");
            }
            if (UsuarioRepo.Get().Any(x => x.Usuario1 == d.Usuario1))
            {
                return BadRequest("Este nombre de usuario no está disponible");
            }
            if (string.IsNullOrEmpty(d.Contraseña))
            {
                return BadRequest("ingrese una contraseña");
            }

            Usuario user = new Usuario()
            {
                Usuario1 = d.Usuario1,
                Contraseña = d.Contraseña,
                Rol = 2

            };
            UsuarioRepo.Insert(user);

            var user1 = UsuarioRepo.Get(user.Id);

            Docente docent = new Docente()
            {
                Nombre = d.Nombre,
                ApellidoMaterno = d.ApellidoMaterno,
                ApellidoPaterno = d.ApellidoPaterno,
                Correo = d.Correo,
                Telefono = d.Telefono,
                Edad = d.Edad,
                TipoDocente = (int)d.TipoDocente,
                IdUsuario = user1.Id

            };
            DocenteRepo.Insert(docent);
            return Ok();
        }

        [HttpPut("EditarDocentes")]
        public IActionResult UpdateDocentes(DocenteDTO d)
        {
            var docentes = DocenteRepo.Get(d.Id);
            var usuario = UsuarioRepo.Get(d.IdUsuario);
            if (docentes == null)
            {
                return NotFound("El docente que quiere editar ya no existe");
            }
            if (string.IsNullOrEmpty(d.Nombre))
            {
                return BadRequest("El nombre no puede ir vacio");

            }
            if (string.IsNullOrEmpty(d.ApellidoMaterno))
            {
                return BadRequest("Ingrese el pimer apellido");

            }
            if (string.IsNullOrEmpty(d.ApellidoMaterno))
            {
                BadRequest("Ingrese el segundo apellido");
            }
            if (string.IsNullOrEmpty(d.Telefono))
            {
                return BadRequest("Ingrese un numero telefonico");

            }
            if (string.IsNullOrEmpty(d.Correo))
            {
                return BadRequest("Ingrese un correo electronico");
            }
            if (d.Edad < 18)
            {
                return BadRequest("EL docente en cuestion debe ser mayor de edad");

            }
            if (string.IsNullOrEmpty(d.Usuario1))
            {
                return BadRequest("Ingrese un nombre de usuario");
            }
            if (UsuarioRepo.Get().Any(x => x.Usuario1 == d.Usuario1 && x.Id != d.IdUsuario))
            {
                return BadRequest("Este nombre de usuario no está disponible");
            }
            if (string.IsNullOrEmpty(d.Contraseña))
            {
                return BadRequest("ingrese una contraseña");
            }
            if (usuario != null)
            {
                docentes.Nombre = d.Nombre;
                docentes.ApellidoMaterno = d.ApellidoMaterno;
                docentes.ApellidoPaterno = d.ApellidoPaterno;
                docentes.Correo = d.Correo;
                docentes.Telefono = d.Telefono;
                docentes.Edad = d.Edad;
                usuario.Usuario1 = d.Usuario1;
                usuario.Contraseña = d.Contraseña;
                UsuarioRepo.Update(usuario);
                DocenteRepo.Update(docentes);
            }




            return Ok();

        }

        [HttpPost("AsignarMateriasEspeciales")]
        public IActionResult AsignarMaterias(DocenteDTOM d)
        {
            var docente = DocenteRepo.Get(d.IdDocente);
            var asignatura = AsigRepo.Get(d.IdAsignatura);
            var maxperiodo = PeriodoRepo.Get().Max(x => x.Id);
            var doAs = DocenteAsignaturaRepo.Get().ToList();
            if (docente == null)
            {
                return NotFound("Este docente no existe");
            }
            if (docente.TipoDocente == 2)
            {
                return BadRequest("Los docentes de grupo no pueden dar materias especiales");
            }

            foreach (var item in doAs)
            {
                if (DocenteGrupoRepo.Get().Any(x => x.IdGrupo == d.IdGrupo && x.IdDocente == item.IdDocente && item.IdAsignatura == d.IdAsignatura))
                {
                    return BadRequest("Ya hay un grupo que tiene un maestro de esta asignatura");
                }
            }


            DocenteAsignatura MateriaAsignada = new DocenteAsignatura()
            {
                IdDocente = d.IdDocente,
                IdAsignatura = d.IdAsignatura
            };

            DocenteAsignaturaRepo.Insert(MateriaAsignada);
            return Ok();
        }

        [HttpPost("AsignarGrupos")]
        public IActionResult AsignarGrupos(DocenteDTOG d)
        {
            var docente = DocenteRepo.Get(d.IdDocente);
            var asignaturaO = AsigRepo.Get().Where(x => x.TipoAsignatura == 1).ToList();
            var maxperiodo = PeriodoRepo.Get().Max(x => x.Id);

            if (docente == null)
            {
                return NotFound("Este docente no existe");
            }
            if (docente.TipoDocente == 1)
            {
                if (DocenteGrupoRepo.Get().Any(x => x.IdGrupo == d.IdGrupo && x.IdDocente == d.IdDocente && x.IdDocenteNavigation.TipoDocente == 1))
                {
                    return BadRequest("Ya tiene este grupo asignado, elija otro");
                }
                DocenteGrupo GrupoDocent = new DocenteGrupo()
                {
                    IdDocente = d.IdDocente,
                    IdGrupo = d.IdGrupo,
                    IdPeriodo = maxperiodo


                };
                DocenteGrupoRepo.Insert(GrupoDocent);
                return Ok();
            }
            if (DocenteGrupoRepo.Get().Any(x => x.IdGrupo == d.IdGrupo && x.IdDocente != d.IdDocente && x.IdDocenteNavigation.TipoDocente == 2))
            {
                return BadRequest("Este grupo ya tiene un maestro");
            }
            if (DocenteGrupoRepo.Get().Any(x => x.IdGrupo == d.IdGrupo && x.IdDocente == d.IdDocente && x.IdDocenteNavigation.TipoDocente == 2))
            {
                return BadRequest("Solo puede tener un grupo asignado por cada docente");
            }
            DocenteGrupo GrupoDocente = new DocenteGrupo()
            {
                IdDocente = d.IdDocente,
                IdGrupo = d.IdGrupo,
                IdPeriodo = maxperiodo
            };
            DocenteGrupoRepo.Insert(GrupoDocente);
            foreach (var asignatura in asignaturaO)
            {
                DocenteAsignatura docenteAsignatura = new()
                {
                    IdAsignatura = asignatura.Id,
                    IdDocente = d.IdDocente
                };

                DocenteAsignaturaRepo.Insert(docenteAsignatura);
            }

            return Ok();
        }

        [HttpGet("Grupos")]
        public IActionResult GetGrupo()
        {
            var docentes = GrupoRepo.Get().OrderBy(x => x.Id).ToList();
            return Ok(docentes);
        }

        [HttpGet("Materias")]
        public IActionResult GetMaterias()
        {
            var docentes = AsigRepo.Get().Where(x => x.TipoAsignatura == 2).OrderBy(x => x.Id).ToList();
            return Ok(docentes);
        }

        [HttpGet("DocenteGrupo")]
        public IActionResult Getdocente()
        {
            var docentes = DocenteGrupoRepo.Get().OrderBy(x => x.Id).ToList();


            List<DocenteGrupocs> docenteGrupos = new List<DocenteGrupocs>();
            foreach (var item in docentes)
            {
                var docente = DocenteRepo.Get().FirstOrDefault(x => x.Id == item.IdDocente);
                var grupo = GrupoRepo.Get().FirstOrDefault(x => x.Id == item.IdGrupo);
                var perido = PeriodoRepo.Get().FirstOrDefault(x => x.Id == item.IdPeriodo);
                if (docente != null && grupo != null && perido != null)
                {
                    DocenteGrupocs d = new DocenteGrupocs()
                    {
                        Id = item.Id,
                        IdGrupo = item.IdGrupo,
                        IdDocente = item.IdDocente,
                        IdPeriodo = item.IdPeriodo,
                        NombreProfe = docente.Nombre,
                        tipoDocente = (TipoDocente)docente.TipoDocente,
                        PrimerApellido = docente.ApellidoPaterno,
                        SegundoApellido = docente.ApellidoMaterno,
                        Grado = grupo.Grado,
                        Seccion = grupo.Seccion,
                        periodo = perido.Año


                    };

                    docenteGrupos.Add(d);
                }





            }



            return Ok(docenteGrupos);
        }

        [HttpGet("DocenteMateria")]

        public IActionResult GetDocenteMateria()
        {
            var docenMaterias = DocenteAsignaturaRepo.Get().Where(x => x.IdAsignaturaNavigation.TipoAsignatura == 2).OrderBy(a => a.Id).ToList();

            List<DocenteAsignaturasDTO> lista = new();
            foreach (var item in docenMaterias)
            {
                var docente = DocenteRepo.Get(item.IdDocente);
                var asigna = AsigRepo.Get().FirstOrDefault(x => x.Id == item.IdAsignatura);
                DocenteAsignaturasDTO dA = new()
                {
                    IdDocente = docente.Id,
                    NombreProfe = docente.Nombre,
                    PrimerApellido = docente.ApellidoPaterno,
                    SegundoApellido = docente.ApellidoMaterno,
                    IdAsignatura = asigna.Id,
                    asignatura = asigna.Nombre



                };
                lista.Add(dA);
            }

            return Ok(lista);
        }


        [HttpGet("Notas")]
        public IActionResult GetNotas()
        {
            var notas = NotasRepo.Get().Include(x => x.IdAlumnoNavigation.IdGrupoNavigation).Select(x => new NotasDTOA()
            { 
                Id= x.Id,
                Titulo=x.Titulo,
                Descripcion = x.Descripcion,
                IdAlumno=x.IdAlumno,
                NombreAlumno=x.IdAlumnoNavigation.Nombre,
               Grado=x.IdAlumnoNavigation.IdGrupoNavigation.Grado,
               Seccion= x.IdAlumnoNavigation.IdGrupoNavigation.Seccion
                
               
            });

            return Ok(notas);
        }

        [HttpPost("AgregarNotas")]
        public IActionResult PostNotas(NotasDTO notas)
        {
            if (string.IsNullOrEmpty(notas.Titulo))
            {
                return BadRequest("El titulo no pued ir vacio");
            }
            if (string.IsNullOrWhiteSpace(notas.Descripcion))
            {
                return BadRequest("La descripcion no puede i vacia");
            }

            Notasdirector n = new Notasdirector()
            {
                Titulo = notas.Titulo,
                Descripcion = notas.Descripcion,
                IdAlumno = notas.IdAlumno


            };
            NotasRepo.Insert(n);
            return Ok();
        }

        [HttpPut("EditarNotas")]
        public IActionResult PutNotas(NotasDTO notas)
        {
            var n = NotasRepo.Get(notas.Id);
            if (string.IsNullOrEmpty(notas.Titulo))
            {
                return BadRequest("El titulo no pued ir vacio");
            }
            if (string.IsNullOrWhiteSpace(notas.Descripcion))
            {
                return BadRequest("La descripcion no puede i vacia");
            }
            if (n == null)
            {
                return BadRequest("La nota quedesea editar ya no existe o ya ha sido eliminada");
            }
            n.Titulo = notas.Titulo;
            n.Descripcion = notas.Descripcion;
            n.IdAlumno= notas.IdAlumno;
            NotasRepo.Update(n);
            return Ok();
        }

        [HttpDelete("EliminarNotas/{id}")]
        public IActionResult DeleteNotas(int id)
        {
            var n = NotasRepo.Get().Find(id);
            if (n == null)
                return BadRequest("La nota que desea eliminar ya no existe");

            NotasRepo.Delete(n);
            return Ok();
        }

        [HttpGet("Alumnos")]
        public IActionResult GetAlumnos()
        {
            var alumnos = alumnoRepo.Get().Include(x=>x.IdGrupoNavigation).Select(x=> new AlumnoDTO()
            {

                Id = x.Id,
                Nombre=x.Nombre,
                Grado=x.IdGrupoNavigation.Grado,
                Seccion=x.IdGrupoNavigation.Seccion
            } ).ToList();
            return Ok(alumnos);
        }

        [HttpGet("Calendario")]
        public IActionResult GetCalendario()
        {
            var evento = CalendarioRepo.Get();
            return Ok(evento);
        }

        [HttpPost("AgregarEvento")]
        public IActionResult PostCalendario(CalendarioDTO calendario)
        {

            if (string.IsNullOrWhiteSpace(calendario.Titulo))
            {
                return BadRequest("El titulo del evento no debe ir vacio");
            }
            if (string.IsNullOrWhiteSpace(calendario.Descripcion))
            {
                return BadRequest("La Descripcion del evento no debe ir vacio");
            }
            Calendario c = new Calendario()
            {
                Fecha = calendario.Fecha,
                Titulo = calendario.Titulo,
                Descripcion = calendario.Descripcion
            };
            CalendarioRepo.Insert(c);
            return Ok();
        }

        [HttpPut("EditarEvento")]
        public IActionResult PutEvento(CalendarioDTO calendario)
        {
            var c = CalendarioRepo.Get().Find(calendario.IdCalendario);

            if (string.IsNullOrWhiteSpace(calendario.Titulo))
            {
                return BadRequest("El titulo del evento no debe ir vacio");
            }
            if (string.IsNullOrWhiteSpace(calendario.Descripcion))
            {
                return BadRequest("La Descripcion del evento no debe ir vacio");
            }
            if (c == null)
            {
                BadRequest("El evento que desea editar ya no existe o ya ha sido eliminado");
            }

            c.Titulo = calendario.Titulo;
            c.Descripcion = calendario.Descripcion;
            c.Fecha = calendario.Fecha;

            CalendarioRepo.Update(c);

            return Ok();
        }

        [HttpDelete("EliminarEvento/{Id}")]
        public IActionResult DeleteEveto(int Id)
        {
            var c = CalendarioRepo.Get().Find(Id);
            if (c == null)
            {
                return BadRequest("El evento que desea eliminar ya ha sido eliminado o ya no existe");
            }
            CalendarioRepo.Delete(c);
            return Ok();
        }


        [HttpDelete("EliminarDocentes/{id}")]
        public IActionResult Delete(int id)
        {

            var objeto= DocenteRepo.Get().FirstOrDefault(x => x.Id == id);


            if (objeto == null)
            {
                return NotFound("Este docente no existe o y ha sido eliminado");
            }
            var registrosTablaSecundaria1 = DocenteGrupoRepo.Get().Where(r => r.IdDocente == id);

            //Buscar los registros asociados en la tabla "TablaSecundaria2"
            var registrosTablaSecundaria2 = DocenteAsignaturaRepo.Get().Where(r => r.IdDocente == id);

            var docenteUsuario = UsuarioRepo.Get().FirstOrDefault(r => r.Id == objeto.IdUsuario);





            //Eliminar los registros asociados en la tabla "TablaSecundaria1"
            DocenteGrupoRepo.Get().RemoveRange(registrosTablaSecundaria1);

            //Eliminar los registros asociados en la tabla "TablaSecundaria2"
            DocenteAsignaturaRepo.Get().RemoveRange(registrosTablaSecundaria2);

            //Guardar los cambios en la base de datos


            //Buscar los registros asociados en la tabla "TablaSecundaria1"


            DocenteRepo.Delete(objeto);

            UsuarioRepo.Get().RemoveRange(docenteUsuario);
            return Ok();

        }
    }

}

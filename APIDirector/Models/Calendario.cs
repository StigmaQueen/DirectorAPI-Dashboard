﻿using System;
using System.Collections.Generic;

namespace APIDirector.Models;

public partial class Calendario
{
    public int IdCalendario { get; set; }

    public string Titulo { get; set; } = null!;

    public DateOnly? Fecha { get; set; }

    public string? Descripcion { get; set; }
}

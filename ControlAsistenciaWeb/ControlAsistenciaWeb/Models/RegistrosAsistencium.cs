using System;
using System.Collections.Generic;

namespace ControlAsistenciaWeb.Models;

public partial class RegistrosAsistencium
{
    public int Id { get; set; }

    public int EmpleadoId { get; set; }

    public DateOnly Fecha { get; set; }

    public string Tipo { get; set; } = null!;

    public string Hora { get; set; } = null!;

    public virtual Empleado Empleado { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ControlAsistenciaWeb.Models;

public partial class TotalesAsistencium
{
    public int Id { get; set; }

    public int EmpleadoId { get; set; }

    public string TotalEntrada { get; set; } = null!;

    public string TotalSalida { get; set; } = null!;

    public virtual Empleado Empleado { get; set; } = null!;
}

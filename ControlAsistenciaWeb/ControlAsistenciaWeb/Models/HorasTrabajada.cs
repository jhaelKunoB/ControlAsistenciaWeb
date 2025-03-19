using System;
using System.Collections.Generic;

namespace ControlAsistenciaWeb.Models;

public partial class HorasTrabajada
{
    public int Id { get; set; }

    public int EmpleadoId { get; set; }

    public string TiempoTotal { get; set; } = null!;

    public string HorasTotales { get; set; } = null!;

    public virtual Empleado Empleado { get; set; } = null!;
}

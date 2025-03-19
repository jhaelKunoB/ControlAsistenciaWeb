using System;
using System.Collections.Generic;

namespace ControlAsistenciaWeb.Models;

public partial class Empleado
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Departamento { get; set; } = null!;

    public virtual ICollection<HorasTrabajada> HorasTrabajada { get; set; } = new List<HorasTrabajada>();

    public virtual ICollection<RegistrosAsistencium> RegistrosAsistencia { get; set; } = new List<RegistrosAsistencium>();
}

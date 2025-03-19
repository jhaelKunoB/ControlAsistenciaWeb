using Microsoft.AspNetCore.Mvc;
using ControlAsistenciaWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace ControlAsistenciaWeb.Controllers
{
	public class EmpleadosController : Controller
	{

		private readonly ControlAsistenciaContext _context;

		public EmpleadosController(ControlAsistenciaContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var usuarios = _context.Empleados
					  .Select(e => new EmpleadoViewModel
					  {
						  Id = e.Id,
						  Nombre = e.Nombre,
						  Departamento = e.Departamento,
						  DiasTrabajados = _context.RegistrosAsistencia
							  .Where(r => r.EmpleadoId == e.Id && r.Tipo == "Entrada" && r.Hora != null)
							  .Select(r => r.Fecha)
							  .Distinct()
							  .Count()
					  })
					  .ToList();

			return View(usuarios);
		}
		//public class EmpleadoViewModel
		//{
		//	public int Id { get; set; }
		//	public string Nombre { get; set; }
		//	public string Departamento { get; set; }
		//	public int DiasTrabajados { get; set; }
		//}

		public IActionResult AsistenciaEmpl(int id)
		{
			var idAsisten = _context.RegistrosAsistencia
				   .Where(e => e.EmpleadoId == id)
				   .Include(r => r.Empleado)
				   .GroupBy(r => new { r.Fecha, r.Empleado.Nombre }) 
				   .Select(g => new
				   {
					   Fecha = g.Key.Fecha,
					   Empleado = g.Key.Nombre,
					   Entrada = g.Where(r => r.Tipo == "Entrada").Select(r => new { r.Hora }).FirstOrDefault(), 
					   Salida = g.Where(r => r.Tipo == "Salida").Select(r => new { r.Hora }).FirstOrDefault()  
				   })
				   .ToList();

			return View(idAsisten);
		}


		[HttpPost]
		public IActionResult DesencriptarHoras([FromBody] HorasDesencriptarRequest request)
		{
			var password = request.Password;
			var horas = request.Horas;
			var resultado = new List<object>();

			try
			{
				foreach (var hora in horas)
				{
					string entradaDesencriptada = null;
					string salidaDesencriptada = null;

					// Validar si Entrada no es null antes de desencriptar
					if (!string.IsNullOrEmpty(hora.Entrada))
					{
						entradaDesencriptada = DecryptAES(hora.Entrada, password);
					}

					// Validar si Salida no es null antes de desencriptar
					if (!string.IsNullOrEmpty(hora.Salida))
					{
						salidaDesencriptada = DecryptAES(hora.Salida, password);
					}

					resultado.Add(new
					{
						Entrada = entradaDesencriptada,
						Salida = salidaDesencriptada
					});
				}

				return Json(resultado);
			}
			catch
			{
				return BadRequest("Error al desencriptar. Verifique la contraseña.");
			}
		}


		public static string DecryptAES(string cipherTextBase64, string password)
		{
			byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

			using (SHA256 sha = SHA256.Create())
			{
				passwordBytes = sha.ComputeHash(passwordBytes);
			}

			using (Aes aes = Aes.Create())
			{
				aes.Key = passwordBytes;
				aes.IV = new byte[16]; // IV en cero, igual que en el cifrado

				using (MemoryStream ms = new MemoryStream())
				using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
				{
					cs.Write(cipherBytes, 0, cipherBytes.Length);
					cs.FlushFinalBlock();

					return Encoding.UTF8.GetString(ms.ToArray());
				}
			}
		}


		public class HorasDesencriptarRequest
		{
			public string Password { get; set; }
			public List<HoraData> Horas { get; set; }
		}
		public class HoraData
		{
			public string Entrada { get; set; }
			public string Salida { get; set; }
		}

	}
}

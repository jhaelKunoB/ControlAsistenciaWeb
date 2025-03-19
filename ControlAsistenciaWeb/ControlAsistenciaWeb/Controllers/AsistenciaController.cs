using ControlAsistenciaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ControlAsistenciaWeb.Controllers
{
	public class AsistenciaController : Controller
	{
		private readonly ControlAsistenciaContext _context;

		public AsistenciaController(ControlAsistenciaContext context)
		{
			_context = context;
		}


		public IActionResult Index()
		{
			var idAsisten = _context.RegistrosAsistencia
				   .Include(r => r.Empleado)
				   .GroupBy(r => new { r.Fecha, r.Empleado.Nombre })
				   .Select(g => new
				   {
					   Fecha = g.Key.Fecha,
					   Empleado = g.Key.Nombre,
					   Entrada = g.Where(r => r.Tipo == "Entrada").Select(r => new { r.Hora }).FirstOrDefault(),
					   Salida = g.Where(r => r.Tipo == "Salida").Select(r => new { r.Hora }).FirstOrDefault()
				   })
				   .OrderByDescending(r => r.Fecha)
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

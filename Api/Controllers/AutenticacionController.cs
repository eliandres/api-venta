using Microsoft.AspNetCore.Mvc;
using Api.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string secretaKey;
        private readonly string cadenaSQL;

        public AutenticacionController(IConfiguration config)
        {
            secretaKey = config.GetSection("settings").GetSection("secretaKey").ToString();
            cadenaSQL = config.GetConnectionString("CadenaConexion");
        }

        [HttpPost]
        [Route("autenticacion")]
        public IActionResult Autenticar([FromBody] Usuario request)
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            string contrasenaEncriptada = EncriptarContrasena(request.Contrasena);
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                conexion.Open();

                // Creo el comando para ejecutar la función
                var cmd = new SqlCommand("SELECT * FROM dbo.ObtenerUsuarioPorUsuario(@NombreUsuario)", conexion);
                cmd.Parameters.AddWithValue("@NombreUsuario", request.NombreUsuario);

                // Ejecutar la consulta y obtener los resultados
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaUsuarios.Add(new Usuario//modelo
                        {
                            IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                            NombreUsuario = reader["NombreUsuario"].ToString(),
                            Contrasena = reader["Contrasena"].ToString(),
                            NombrePersonal = reader["NombrePersonal"].ToString(),
                            Telefono = reader["Telefono"].ToString(),
                            CorreoElectronico = reader["CorreoElectronico"].ToString()
                        });
                    }
                }
            }

            if (listaUsuarios.Count > 0)
            {
                var usuario = listaUsuarios[0]; // Tomar el primer usuario encontrado (debería ser único por usuario/correo)
                if (usuario.Contrasena == contrasenaEncriptada)
                {
                    var secreByter = Encoding.UTF8.GetBytes(secretaKey);
                    var claims = new ClaimsIdentity();
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.NombreUsuario));
                    claims.AddClaim(new Claim(ClaimTypes.Actor, usuario.NombrePersonal));
                    claims.AddClaim(new Claim(ClaimTypes.Name, Convert.ToString(usuario.IdUsuario)));
                    claims.AddClaim(new Claim(ClaimTypes.MobilePhone, usuario.Telefono));
                    claims.AddClaim(new Claim(ClaimTypes.PostalCode, usuario.CorreoElectronico));
                    var token = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddMinutes(60),//asignar 1 hora para que el token expire
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secreByter), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHd = new JwtSecurityTokenHandler();
                    var tokenConfig = tokenHd.CreateToken(token);
                    var tokenCreado = tokenHd.WriteToken(tokenConfig);

                    return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado });
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new { token = "Denegado" });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "Correo/Usuario no válido" });
            }
            
        }

        [HttpPost]//esto es validacion
        public string EncriptarContrasena(string contrasena)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(contrasena));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }


}
}

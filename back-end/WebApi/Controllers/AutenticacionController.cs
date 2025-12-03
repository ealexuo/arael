using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using Qfile.Core.Tipos;
using WebApi.Modelos;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly ApplicationSettingsModelo _appSetings;
        private readonly IAutenticacionServicio _servicio;
        private readonly IUsuarioServicio _usuarioServicio;

        public AutenticacionController(IOptions<ApplicationSettingsModelo> appSettings, IAutenticacionServicio servicio, IUsuarioServicio usuarioServicio)
        {
            _appSetings = appSettings.Value;
            _servicio = servicio;
            _usuarioServicio = usuarioServicio;
        }

        // GET: api/Usuario/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModelo loginModelo)
        {
            try
            {
                int resultadoCheckPassword = ValidaPasswordTipos.Invalido;
                var usuario = await _usuarioServicio.ObtenerPorNombreUsuarioAsync(loginModelo.NombreUsuario);

                if (usuario != null)
                {
                    resultadoCheckPassword = await _servicio.ValidaPasswordAsync(usuario.IdUsuario, loginModelo.Password);
                }

                if (usuario == null || resultadoCheckPassword == ValidaPasswordTipos.Invalido)
                    return StatusCode(400);

                if (resultadoCheckPassword == ValidaPasswordTipos.Valido)
                {
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[] {
                    new Claim("IdUsuario", usuario.IdUsuario.ToString(), null),
                    new Claim("IdEntidad", usuario.IdEntidad.ToString(), null),
                    new Claim("IdUser", usuario.IdUsuario.ToString(), null),
                    new Claim("IdEntity", usuario.IdEntidad.ToString(), null)
                }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSetings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature),
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);

                    await _servicio.RegistrarLogeo(usuario.IdEntidad, usuario.IdUsuario);

                    return Ok(new { token });
                }
                else if (resultadoCheckPassword == ValidaPasswordTipos.ValidoRequiereCambio)
                {
                    return Ok(new { token = "RequiereCambioPassword" });
                }
                else
                {
                    return BadRequest(new { message = "Usuario o password incorrecto." });
                }
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        // Post: api/Usuario/CambiarPassword
        //[Authorize]
        [HttpPost("CambiarPassword")]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordModelo modelo)
        {
            try
            {
                UsuarioModelo usuario = await _usuarioServicio.ObtenerPorNombreUsuarioAsync(modelo.NombreUsuario);
                var resultadoCheckPassword = await _servicio.ValidaPasswordAsync(usuario.IdUsuario, modelo.PasswordActual);

                if (resultadoCheckPassword == ValidaPasswordTipos.ValidoRequiereCambio)
                {
                    await _servicio.CambiarPasswordAsync(usuario.IdUsuario, modelo.PasswordNuevo);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[] {
                    new Claim("IdUsuario", usuario.IdUsuario.ToString(), null),
                    new Claim("IdEntidad", usuario.IdEntidad.ToString(), null)
                }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSetings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature),
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);

                    await _servicio.RegistrarLogeo(usuario.IdEntidad, usuario.IdUsuario);

                    return Ok(new { token });
                }
                else
                {
                    return BadRequest(new { message = "Usuario o password incorrecto." });
                }
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

    }
}

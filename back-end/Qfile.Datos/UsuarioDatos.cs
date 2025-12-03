using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Data;

namespace Qfile.Datos
{
    public class UsuarioDatos : IUsuarioDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public UsuarioDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public async Task<UsuarioModelo> ObtenerPorNombreUsuarioAsync(string identificacionPersonal, string correoElectronico)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                    SELECT ID_ENTIDAD IdEntidad, ID_USUARIO IdUsuario, NO_IDENTIFICACION_PERSONAL NoIdentificacionPersonal, CORREO_ELECTRONICO CorreoElectronico, PRIMER_NOMBRE PrimerNombre, 
                        SEGUNDO_NOMBRE SegundoNombre, OTROS_NOMBRES OtrosNombres, PRIMER_APELLIDO PrimerApellido, SEGUNDO_APELLIDO SegundoApellido, APELLIDO_CASADA ApellidoCasada, TITULO Titulo, 
                        CARGO Cargo, EXTENSION Extension, TELEFONO Telefono, ACTIVO Activo, GENERO Genero, PASSWORD Password, ID_ESTADO IdEstado, FECHA_FIN_INHABILITACION FechaFinInhabilitacion, 
                        ID_UNIDAD_ADMINISTRATIVA IdUnidadAdministrativa, REQUIERE_CAMBIO_PASSWORD RequiereCambioPassword, FECHA_REGISTRO FechaRegistro, USUARIO_REGISTRO UsuarioRegistro, ID_IDIOMA IdIdioma
                    FROM AD_USUARIOS
                    WHERE NO_IDENTIFICACION_PERSONAL = @identificacionPersonal
                        OR CORREO_ELECTRONICO = @correoElectronico";

                var result = await connection.QueryAsync<UsuarioModelo>(sqlQuery, new { identificacionPersonal, correoElectronico });
                return result.FirstOrDefault();
            }
        }
       
        public async Task<UsuarioModelo> ObtenerPorIdAsync(int idUsuario)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                    SELECT ID_ENTIDAD IdEntidad, ID_USUARIO IdUsuario, NO_IDENTIFICACION_PERSONAL NoIdentificacionPersonal, CORREO_ELECTRONICO CorreoElectronico, PRIMER_NOMBRE PrimerNombre, 
                        SEGUNDO_NOMBRE SegundoNombre, OTROS_NOMBRES OtrosNombres, PRIMER_APELLIDO PrimerApellido, SEGUNDO_APELLIDO SegundoApellido, APELLIDO_CASADA ApellidoCasada, TITULO Titulo,
                        CARGO Cargo, EXTENSION Extension, TELEFONO Telefono, ACTIVO Activo, GENERO Genero, PASSWORD Password, ID_ESTADO IdEstado, FECHA_FIN_INHABILITACION FechaFinInhabilitacion,
                        ID_UNIDAD_ADMINISTRATIVA IdUnidadAdministrativa, REQUIERE_CAMBIO_PASSWORD RequiereCambioPassword, FECHA_REGISTRO FechaRegistro, USUARIO_REGISTRO UsuarioRegistro, ID_IDIOMA IdIdioma
                    FROM AD_USUARIOS
                    WHERE ID_USUARIO = @idUsuario";

                var result = await connection.QueryAsync<UsuarioModelo>(sqlQuery, new { idUsuario });
                return result.FirstOrDefault();
            }
        }

        public async Task<int> CrearUsuarioAsync(UsuarioModelo usuario, String password, DateTime fechaRegistro, int idUsuarioRegistro)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string insertarUsuarioSQL = @"                
                INSERT INTO AD_USUARIOS
                (
                    ID_ENTIDAD, ID_USUARIO, NO_IDENTIFICACION_PERSONAL, CORREO_ELECTRONICO, PRIMER_NOMBRE, SEGUNDO_NOMBRE, OTROS_NOMBRES, PRIMER_APELLIDO, SEGUNDO_APELLIDO, 
                    APELLIDO_CASADA, TITULO, CARGO, EXTENSION, TELEFONO, ACTIVO, GENERO, PASSWORD, ID_ESTADO, ID_UNIDAD_ADMINISTRATIVA, REQUIERE_CAMBIO_PASSWORD, 
                    FECHA_REGISTRO, USUARIO_REGISTRO, ID_IDIOMA
                )
                VALUES(
                    @IdEntidad, (select ISNULL(MAX(id_usuario), 0) from ad_usuarios) + 1, @NoIdentificacionPersonal, @CorreoElectronico, @PrimerNombre, @SegundoNombre, @OtrosNombres, @PrimerApellido, @SegundoApellido,
                    @ApellidoCasada, @Titulo, @Cargo, @Extension, @Telefono, @Activo, @Genero, @Password, @IdEstado, @IdUnidadAdministrativa, @RequiereCambioPassword,
                    @FechaRegistro, @UsuarioRegistro, @IdIdioma
                )";

                string obtenerIdUsuarioSQL = "SELECT ID_USUARIO FROM AD_USUARIOS WHERE NO_IDENTIFICACION_PERSONAL = @NoIdentificacionPersonal";

                string asignarRolesSQL = @"INSERT INTO AD_ROLES_USUARIO (ID_ROL, ID_ENTIDAD, ID_USUARIO, FECHA_REGISTRO)
                VALUES(@IdRol, @IdEntidad, @IdUsuario, @FechaRegistro)";

                using (var trx = connection.BeginTransaction())
                {
                    // Obtener información del usuario registro
                    var usuarioRegistro = await ObtenerPorIdAsync(idUsuarioRegistro);

                    // Guardar usuario
                    await connection.ExecuteAsync(insertarUsuarioSQL, new
                    {
                        usuarioRegistro.IdEntidad,
                        usuario.NoIdentificacionPersonal,
                        usuario.CorreoElectronico,
                        usuario.PrimerNombre,
                        usuario.SegundoNombre,
                        usuario.OtrosNombres,
                        usuario.PrimerApellido,
                        usuario.SegundoApellido,
                        usuario.ApellidoCasada,
                        usuario.Titulo,
                        usuario.Cargo,
                        usuario.Extension,
                        usuario.Telefono,
                        Activo = 0,
                        usuario.Genero,
                        Password = password,
                        IdEstado = 1,
                        //FechaFinInhabilitacion = DBNull.Value,
                        usuario.IdUnidadAdministrativa,
                        RequiereCambioPassword = 1,
                        FechaRegistro = fechaRegistro,
                        UsuarioRegistro = idUsuarioRegistro,
                        usuarioRegistro.IdIdioma
                    }, trx);

                    // Obtener Id del usuario
                    usuario.IdUsuario = await connection.QuerySingleAsync<int>(obtenerIdUsuarioSQL, new { 
                        usuario.NoIdentificacionPersonal
                    }, trx);

                    // Asignar Roles
                    foreach(int idRol in usuario.ListaRoles)
                    {
                        await connection.ExecuteAsync(asignarRolesSQL, new
                        {
                            IdRol = idRol,
                            usuarioRegistro.IdEntidad,
                            usuario.IdUsuario,
                            FechaRegistro = fechaRegistro
                        }, trx);
                    }

                    trx.Commit();
                }

                return usuario.IdUsuario;
            }
        }

        public async Task<int> EditarUsuarioAsync(UsuarioModelo usuario, DateTime fechaRegistro)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string editarUsuarioSQL = @"
                    UPDATE AD_USUARIOS
                    SET 
	                    CORREO_ELECTRONICO = @CorreoElectronico, 
	                    PRIMER_NOMBRE = @PrimerNombre, 
	                    SEGUNDO_NOMBRE = @SegundoNombre, 
	                    OTROS_NOMBRES = @OtrosNombres, 
	                    PRIMER_APELLIDO = @PrimerApellido, 
	                    SEGUNDO_APELLIDO = @SegundoApellido, 
	                    APELLIDO_CASADA = @ApellidoCasada, 
	                    TITULO = @Titulo, 
	                    CARGO = @Cargo, 
	                    EXTENSION = @Extension, 
	                    TELEFONO = @Telefono, 
	                    GENERO = @Genero, 
	                    ID_UNIDAD_ADMINISTRATIVA = @IdUnidadAdministrativa,
                        ACTIVO = @Activo
                    WHERE ID_ENTIDAD=@IdEntidad AND ID_USUARIO=@IdUsuario";

                string eliminarRolesActualesSQL = @"DELETE AD_ROLES_USUARIO WHERE ID_ENTIDAD = @IdEntidad AND ID_USUARIO = @IdUsuario";

                string asignarRolesSQL = @"INSERT INTO AD_ROLES_USUARIO (ID_ROL, ID_ENTIDAD, ID_USUARIO, FECHA_REGISTRO)
                VALUES(@IdRol, @IdEntidad, @IdUsuario, @FechaRegistro)";

                using (var trx = connection.BeginTransaction())
                {

                    // Se actualiza el usuario
                    await connection.ExecuteAsync(editarUsuarioSQL, new
                    {
                        usuario.CorreoElectronico,
                        usuario.PrimerNombre,
                        usuario.SegundoNombre,
                        usuario.OtrosNombres,
                        usuario.PrimerApellido,
                        usuario.SegundoApellido,
                        usuario.ApellidoCasada,
                        usuario.Titulo,
                        usuario.Cargo,
                        usuario.Extension,
                        usuario.Telefono,
                        usuario.Genero,
                        usuario.IdUnidadAdministrativa,
                        usuario.IdEntidad,
                        usuario.IdUsuario,
                        usuario.Activo
                    }, trx);

                    // Se eliminan los roles actuales
                    await connection.ExecuteAsync(eliminarRolesActualesSQL, new
                    {
                        usuario.IdEntidad,
                        usuario.IdUsuario
                    }, trx);

                    // Asignar Roles
                    foreach (int idRol in usuario.ListaRoles)
                    {
                        await connection.ExecuteAsync(asignarRolesSQL, new
                        {
                            IdRol = idRol,
                            usuario.IdEntidad,
                            usuario.IdUsuario,
                            FechaRegistro = fechaRegistro
                        }, trx);
                    }

                    trx.Commit();
                }

                return usuario.IdUsuario;
            }
        }

        public async Task<List<UsuarioListaModelo>> ObtenerUsuariosAsync(int pagina, int cantidad, string buscarTexto)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                SELECT
                    us.ID_ENTIDAD AS IdEntidad,
                    us.ID_USUARIO AS IdUsuario,
                    us.NO_IDENTIFICACION_PERSONAL AS NoIdentificacionPersonal,
                    TRIM(CONCAT(us.PRIMER_NOMBRE, ' ', us.SEGUNDO_NOMBRE, ' ', us.PRIMER_APELLIDO, ' ', us.SEGUNDO_APELLIDO)) AS Nombre,
                    us.CORREO_ELECTRONICO AS CorreoElectronico,
                    ua.NOMBRE AS NombreUnidadAdministrativa,
                    us.ACTIVO AS Estado,
                    us.FECHA_REGISTRO,
                    COUNT(*) OVER () CantidadTotal
                FROM AD_USUARIOS us
                    JOIN AD_UNIDADES_ADMINISTRATIVAS ua
                    ON us.ID_UNIDAD_ADMINISTRATIVA = ua.ID_UNIDAD_ADMINISTRATIVA
                WHERE @BuscarTexto = '*'
                    OR us.NO_IDENTIFICACION_PERSONAL LIKE '%' + @BuscarTexto + '%'
                    OR CONCAT(us.PRIMER_NOMBRE ,' ' ,us.SEGUNDO_NOMBRE , ' ' , us.PRIMER_APELLIDO , ' ' , us.SEGUNDO_APELLIDO) LIKE '%' + @BuscarTexto + '%'
                    OR us.CORREO_ELECTRONICO LIKE '%'+ @BuscarTexto +'%'
                    OR ua.NOMBRE LIKE '%'+ @BuscarTexto + '%'
                ORDER BY UPPER(Nombre) 
                OFFSET (@Pagina-1)*@Cantidad ROWS
                FETCH NEXT @Cantidad ROWS ONLY";

                if (String.IsNullOrEmpty(buscarTexto))
                    buscarTexto = "*";

                var result = await connection.QueryAsync<UsuarioListaModelo>(sqlQuery, new { 
                    Pagina = pagina,
                    Cantidad = cantidad,
                    BuscarTexto = buscarTexto
                });

                return result.ToList();
            }
        }

        public async Task<bool> EliminarUsuarioAsync(int idEntidad, int idUsuario)
        {
            //TODO@ verificar si es eliminacion logica

            using (var connection = await connectionProvider.OpenAsync())
            {

                string eliminarRolesActualesSQL = @"DELETE AD_ROLES_USUARIO WHERE ID_ENTIDAD = @IdEntidad AND ID_USUARIO = @IdUsuario";
                string eliminarUsuarioSQL = @"DELETE AD_USUARIOS WHERE ID_ENTIDAD = @idEntidad AND ID_USUARIO = @idUsuario";

                using (var trx = connection.BeginTransaction())
                {
                    // Se eliminan los roles actuales
                    await connection.ExecuteAsync(eliminarRolesActualesSQL, new
                    {
                        idEntidad,
                        idUsuario
                    }, trx);

                    // Se elimina el usuario
                    await connection.ExecuteAsync(eliminarUsuarioSQL, new
                    {
                        idEntidad,
                        idUsuario
                    }, trx);

                    trx.Commit();
                }
                return true;
            }
        }

        public async Task<UsuarioModelo> ObtenerUsuarioAsync(int idUsuario)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string obtenerUsuarioSQL = @"SELECT 
	                                    ID_ENTIDAD IdEntidad,
	                                    ID_USUARIO IdUsuario,
	                                    NO_IDENTIFICACION_PERSONAL NoIdentificacionPersonal,
	                                    CORREO_ELECTRONICO CorreoElectronico,
	                                    PRIMER_NOMBRE PrimerNombre,
	                                    SEGUNDO_NOMBRE SegundoNombre,
	                                    OTROS_NOMBRES OtrosNombres,
	                                    PRIMER_APELLIDO PrimerApellido,
	                                    SEGUNDO_APELLIDO SegundoApellido,
	                                    APELLIDO_CASADA ApellidoCasada,
	                                    TITULO Titulo,
	                                    CARGO Cargo,
	                                    EXTENSION Extension,
	                                    TELEFONO Telefono,
	                                    GENERO Genero,
                                        Activo Activo,
	                                    ID_ESTADO IdEstado,
	                                    ID_UNIDAD_ADMINISTRATIVA IdUnidadAdministrativa
                                    FROM AD_USUARIOS au WHERE ID_USUARIO = @IdUsuario";

                string obtenerIdRolesUsuario = @" SELECT ID_ROL FROM AD_ROLES_USUARIO aru WHERE ID_USUARIO = @IdUsuario";

                var resultadoUsuario = await connection.QueryAsync<UsuarioModelo>(obtenerUsuarioSQL, new
                {
                    IdUsuario = idUsuario
                });

                var usuario = resultadoUsuario.FirstOrDefault();

                var resultadoRoles = await connection.QueryAsync<int>(obtenerIdRolesUsuario, new
                {
                    IdUsuario = idUsuario
                });

                usuario.ListaRoles = resultadoRoles.ToList();

                return usuario;
            }
        }
    }
}
export type Fase = {
  idEntidad: number;
  idProceso: number;
  idFase: number;
  idTipoFase: number;
  tipoFase: string;
  idUnidadAdministrativa: number;
  unidadAdministrativa: string;
  nombre: string;
  descripcion: string;
  tiempoPromedio: number; // decimal in C# maps to number in TypeScript
  idUnidadMedida: number;
  unidadMedida: string;
  asignacionObligatoria: boolean;
  activa: boolean;
  acuseRecibido: boolean;
  idTipoAcceso: number;
  tipoAcceso: string;
  usuarioRegistro: number;
  fechaRegistro: Date;
  listaTransiciones?: Transicion[];
};

export type TiposFase = {
  idTipoFase: number,
  nombre: string
}

export type UsuarioFase = {
  idEntidad: number;
  idProceso: number;
  idFase: number;
  idUsuario: number;
  recepcionTraslado: boolean;
  nombre: string;
  usuarioRegistro: number;
  fechaRegistro: Date;
};

export type Transicion = {
  idEntidad: number;
  idProceso: number;
  idFaseOrigen: number;
  faseOrigen: string;
  unidadAdministrativaFO: string;
  idFaseDestino: number;
  faseDestino: string;
  unidadAdministrativaFD: string;
  activa: boolean;
  usuarioRegistro: number;
  fechaRegistro: Date;
};

export type TransicionNotificacion = {
  idEntidad: number;
  idProceso: number;
  idFaseOrigen: number;
  faseOrigen: string;
  idFaseDestino: number;
  faseDestino: string;
  correo: string;
  usuarioRegistro: number;
  fechaRegistro: Date;
};

export type RequisitoPorTransicion = {
  idEntidad: number;
  idProceso: number;
  idFaseOrigen: number;
  idFaseDestino: number;
  idRequisito: number;
  idTipoCampo: number;
  nombreTipoCampo: string;
  campo: string;
  obligatorio: boolean;
  fechaRegistro: Date;
  usuarioRegistro: number;
};
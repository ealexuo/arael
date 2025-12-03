
export type Plantilla = {
    idEntidad: number;
    idProceso: number;
    idPlantilla: number;
    nombre: string;
    descripcion: string;
    version: number;
    versionPropuesta: number;
    activa: boolean;
    listaSecciones?: Seccion[];
};

export type Seccion = {
    idEntidad: number;
    idProceso: number;
    idPlantilla: number;
    idSeccion: number;
    nombre: string;
    descripcion: string;
    orden: number;
    activa: boolean;
    listaCampos?: any[]; // Todo Agregar tipo
}

export type Campo = {
    idEntidad: number;
    idProceso: number;
    idPlantilla: number;
    idSeccion: number;
    idCampo: number;
    nombre: string;
    descripcion: string;
    orden: number;
    longitud: number;
    obligatorio: boolean;
    noColumnas: number;
    idTipoCampo: number;
    activo: boolean;
    idCampoPadre: number;
    nombreCampoPadre: string;
}

export type HistoricoPlantillas = {
    idEntidad: number;
    idProceso: number;
    idPlantilla: number;
    version: number;
    fechaPublicacion: Date;
};

export type ValorLista = {
  idEntidad: number;
  idProceso: number;
  idPlantilla: number;
  idSeccion: number;
  idCampo: number;
  idValor: number;
  orden: number;
  idCampoPadre: number;
  idValorPadre: number;
  nombre: string;
  predeterminado: boolean;
  seleccionado?: boolean;
};


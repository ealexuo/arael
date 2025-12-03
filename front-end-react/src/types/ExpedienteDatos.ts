export type ExpedienteSeccionDatos = {
    idExpediente: number;
    idPlantilla: number;
    idSeccion: number;
    nombre: string;
    activa: boolean;
    listaCampos: ExpedienteCampoDatos[];
}

export type ExpedienteCampoDatos = {
    idCampo: number;
    orden: number;
    nombre: string;
    descripcion: string;
    longitud: string;
    obligatorio: boolean;
    noColumnas: number;
    idCampoPadre: number;
    valor: string;
    idTipoCampo: number;
    activo: boolean;
}
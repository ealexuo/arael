
export type Proceso = {
    idEntidad: number;
    idProceso: number;
    nombre: string;
    descripcion: string;
    color: string;
    idTipoProceso: number;
    tipoProceso: string;
    idUnidadAdministrativa: number;
    unidadAdministrativa: string;
    siglasUA: string;
    estado: boolean;
    expedientesActivos: number;
    expedientesFinalizados: number;
    totalExpedientes: number;
};
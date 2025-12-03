import axiosService from "../axios/axiosService";

const BASE_PATH = "api/Expediente/";

export const fileService = {
  getAll: async (offset: number, fetch: number, searchText: string): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH, {
      params: {
        Pagina: offset.toString(),
        Cantidad: fetch.toString(),
        BuscarTexto: searchText,
      },
    });
  },
   add: async (file: any): Promise<any> => {
     return await axiosService.post(BASE_PATH, file);
   },
   unificarExpedientes: async (files: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + "UnificarExpedientes/", files);
  },
  copiarExpediente: async (copyFile: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + "CopiarExpediente/", copyFile);
  },
  agregarAnotacion: async (anotationFile: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + "Anotaciones/", anotationFile);
  },
  obtenerAnotaciones: async (anotationFile: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + "Anotaciones/ObtenerAnotaciones", anotationFile);
  },
  eliminarAnortacion: async (anotationFile: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + "Anotaciones/EliminarAnotacion", anotationFile);
  },
  obtenerFasesSiguientes: async (idExpediente: number): Promise<any> => {
    return await axiosService.get(BASE_PATH + "ObtenerFasesTraslado/" + idExpediente);
  },
  // edit: async (workflow: any): Promise<any> => {
  //   return await axiosService.put(BASE_PATH, workflow);
  // },
  get: async (fileId: number): Promise<any> => {
    return await axiosService.get(BASE_PATH + fileId);
  },
  getData: async (fileId: number): Promise<any> => {
    return await axiosService.get(BASE_PATH + "ObtenerDatos/" + fileId);
  },
  trasladarExpediente: async (file: any): Promise<any> => {
    console.log("Si llega", file);
    return await axiosService.post(BASE_PATH + "TrasladarExpediente", file);
  },
  // delete: async (workflowId: number): Promise<any> => {
  //   return await axiosService.delete(BASE_PATH + workflowId);
  // } 
};

import axiosService from "../axios/axiosService";

const BASE_PATH = "api/Procesos/";

export const workflowService = {
  getAll: async (offset: number, fetch: number, searchText: string): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH, {
      params: {
        Pagina: offset.toString(),
        Cantidad: fetch.toString(),
        BuscarTexto: searchText,
      },
    });
  },
  add: async (workflow: any): Promise<any> => {
    return await axiosService.post(BASE_PATH, workflow);
  },
  edit: async (workflow: any): Promise<any> => {
    return await axiosService.put(BASE_PATH, workflow);
  },
  get: async (workflowId: number): Promise<any> => {
    return await axiosService.get(BASE_PATH + workflowId);
  },
  delete: async (workflowId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + workflowId);
  },
  obtenerProcesosActivos: async (): Promise<any> => {
    return await axiosService.get(BASE_PATH + 'ObtenerProcesosActivos');
  }
};

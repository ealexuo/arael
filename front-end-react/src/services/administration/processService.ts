import axiosService from "../axios/axiosService";

const BASE_PATH = "api/Process/";

export const processService = {
  getAll: async (offset: number, fetch: number, searchText: string): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH, {
      params: {
        Pagina: offset.toString(),
        Cantidad: fetch.toString(),
        BuscarTexto: searchText,
      },
    });
  },
};
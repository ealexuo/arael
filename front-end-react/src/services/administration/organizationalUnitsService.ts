import axiosService from "../axios/axiosService";

const BASE_PATH = "api/UnidadAdministrativa/";

export const organizationalUnitsService = {
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
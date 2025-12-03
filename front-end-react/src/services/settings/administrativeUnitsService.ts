import axiosService from "../axios/axiosService";

const BASE_PATH = "api/UnidadAdministrativa/";

export const administrativeUnitsService = {
  getAll: async (): Promise<any> => {
    return await axiosService.get<any>(BASE_PATH);
  },
  getAll2: async (offset: number, fetch: number, searchText: string): Promise<any> => {
    return await axiosService.get<any>(BASE_PATH, {
      params: {
        Pagina: offset.toString(),
        Cantidad: fetch.toString(),
        BuscarTexto: searchText,
      },
    });
  },
  add: async (administrativeUnitId: any): Promise<any> => {
    return await axiosService.post(BASE_PATH, administrativeUnitId);
  },
  edit: async (administrativeUnitId: any): Promise<any> => {
    return await axiosService.put(BASE_PATH, administrativeUnitId);
  },
  get: async (administrativeUnitId: number): Promise<any> => {
    return await axiosService.get(BASE_PATH + administrativeUnitId);
  },
  delete: async (administrativeUnitId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + administrativeUnitId);
  }
};



// import axiosService from "../axios/axiosService";

// const BASE_PATH = "api/UnidadAdministrativa/";

// export const administrativeUnitsService = {
//   getAll: async (offset: number, fetch: number, searchText: string): Promise<any> => {
//     return await axiosService.get<any>(BASE_PATH, {
//       params: {
//         Pagina: offset.toString(),
//         Cantidad: fetch.toString(),
//         BuscarTexto: searchText,
//       },
//     });
//   },
//   add: async (origin: any): Promise<any> => {
//     return await axiosService.post(BASE_PATH, origin);
//   },
//   edit: async (origin: any): Promise<any> => {
//     return await axiosService.put(BASE_PATH, origin);
//   },
//   get: async (originId: number): Promise<any> => {
//     return await axiosService.get(BASE_PATH + originId);
//   },
//   delete: async (originId: number): Promise<any> => {
//     return await axiosService.delete(BASE_PATH + originId);
//   }
// };

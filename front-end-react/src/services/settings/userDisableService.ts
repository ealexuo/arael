import axiosService from "../axios/axiosService";

const BASE_PATH = "api/Inhabilitacion/";

export const userDisableService = {
  
  getAll: async (userId: number): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH + 'ObtenerInhabilitacionesUsuario/' + userId);
  },
  add: async (userDisableObject: any): Promise<any> => {
    return await axiosService.post(BASE_PATH, userDisableObject);
  },
  edit: async (userDisableObject: any): Promise<any> => {
    return await axiosService.put(BASE_PATH, userDisableObject);
  },  
  delete: async (userDisableObject: any): Promise<any> => {    
    return await axiosService.delete(BASE_PATH + userDisableObject.idUsuario + '/' + userDisableObject.idHistoricoInhabilitacion);
  }
  
};
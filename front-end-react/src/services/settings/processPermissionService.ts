import axiosService from "../axios/axiosService";

const BASE_PATH = "api/ProcesoPermiso/";

export const processPermissionService = {
  
  get: async (userId: any): Promise<any> => {
    return await axiosService.get(BASE_PATH + userId);
  },
  edit: async (processPermissionList: any, selectedUser: any): Promise<any> => {
    return await axiosService.post(BASE_PATH, {usuario: selectedUser, listaProcesosPermisos: processPermissionList});
  }
  
};
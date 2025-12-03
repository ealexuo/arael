import axiosService from "../axios/axiosService";

const BASE_PATH = "api/RequisitosCreacion/";

export const workflowRequirementService = {
  getAll: async (processId: number): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH + 'ObtenerRequisitos/' + processId);
  },  
  add: async (requirement: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'Requisito', requirement);
  },
  edit: async (requirement: any): Promise<any> => {
    return await axiosService.put(BASE_PATH, requirement);
  },  
  delete: async (requirement: any): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Requisito/' + requirement.idProceso + '/' + requirement.idRequisito);
  }
};




import axiosService from "../axios/axiosService";

const BASE_PATH = "api/FasesTransiciones/";

export const workflowPhaseService = {
  getAll: async (processId: number): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH + 'ObtenerFases/' + processId);
  },  
  add: async (phase: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'Fase', phase);
  },
  edit: async (phase: any): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'Fase', phase);
  },  
  delete: async (phase: any): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Fase/' + phase.idProceso + '/' + phase.idFase);
  },
  getPhaseTypes: async (): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH + 'ObtenerTiposFases/');
  },
  getPhaseAccessTypes: async (): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH + 'ObtenerTiposAccesos/');
  },
  getMeasurementUnits: async (): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH + 'ObtenerUnidadesMedida/');
  },
  getPhaseUsers: async (workflowId: number, phaseId: number): Promise<any> => {
    return await axiosService.get(BASE_PATH + 'UsuariosFase/' + workflowId + '/' + phaseId);
  },
  addPhaseUser: async (phaseUser: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'UsuariosFase', phaseUser);
  },
  deletePhaseUser: async (workflowId: number, phaseId: number, userId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'UsuariosFase/' + workflowId + '/' + phaseId + '/' + userId);
  },
  updateReceptionPhaseUser: async (phaseUser: any): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'UsuariosFase', phaseUser);
  },
  addPhaseTransition: async (phaseTransition: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'Transiciones', phaseTransition);
  },
  deletePhaseTransition: async (workflowId: number, originPhaseId: number, destinationPhaseId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Transiciones/' + workflowId + '/' + originPhaseId + '/' + destinationPhaseId);
  },
  activatePhaseTransition: async (phaseTransition: any): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'Transiciones', phaseTransition);
  },
  getPhaseTransitionUsers: async (workflowId: number, originPhaseId: number, destinationPhaseId: number): Promise<any> => {
    return await axiosService.get(BASE_PATH + 'TransicionUsuarios/' + workflowId + '/' + originPhaseId + '/' + destinationPhaseId);
  },
  addPhaseTransitionUser: async (transitionUser: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'TransicionUsuarios', transitionUser);
  },
  deletePhaseTransitionUser: async (workflowId: number, originPhaseId: number, destinationPhaseId: number, userId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'TransicionUsuarios/' + workflowId + '/' + originPhaseId + '/' + destinationPhaseId + '/' + userId);
  },
  getPhaseTransitionNotifications: async (workflowId: number, originPhaseId: number, destinationPhaseId: number): Promise<any> => {
    return await axiosService.get(BASE_PATH + 'TransicionNotificaciones/' + workflowId + '/' + originPhaseId + '/' + destinationPhaseId);
  },
  addPhaseTransitionNotification: async (transitionNotification: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'TransicionNotificaciones', transitionNotification);
  },
  deletePhaseTransitionNotification: async (workflowId: number, originPhaseId: number, destinationPhaseId: number, email: string): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'TransicionNotificaciones/' + workflowId + '/' + originPhaseId + '/' + destinationPhaseId + '/' + email);
  },
  getPhaseTransitionRequieriments: async (workflowId: number, originPhaseId: number, destinationPhaseId: number): Promise<any> => {
    return await axiosService.get(BASE_PATH + 'RequisitosPorTransicion/' + workflowId + '/' + originPhaseId + '/' + destinationPhaseId);
  },
  addPhaseTransitionRequieriment: async (transitionRequieriment: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'RequisitosPorTransicion', transitionRequieriment);
  },
  deletePhaseTransitionRequieriment: async (workflowId: number, originPhaseId: number, destinationPhaseId: number, requierimentId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'RequisitosPorTransicion/' + workflowId + '/' + originPhaseId + '/' + destinationPhaseId + '/' + requierimentId);
  },
};




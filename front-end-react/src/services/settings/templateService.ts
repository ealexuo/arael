import { Seccion, Campo, ValorLista } from "../../types/Plantilla";
import axiosService from "../axios/axiosService";

const BASE_PATH = "api/Plantillas/";

export const templateService = {
  getAll: async (workflowId: any): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH + 'ObtenerPlantillas/' + workflowId, {});
  },
  getSpecificTamplate: async (workflowId: any, templateId: any): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH + 'ObtenerPlantillaActual/' + workflowId + '/' + templateId, {});
  },
  add: async (template: any): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'Plantilla', template);
  },
  edit: async (template: any): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'Plantilla', template);
  },
  // get: async (workflowId: number): Promise<any> => {
  //   return await axiosService.get(BASE_PATH + workflowId);
  // },
  delete: async (workflowId: number, templateId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Plantilla/' + workflowId + '/' + templateId);
  },
  // Sections
  addSection: async (section: Seccion): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'Seccion', section);
  },
  editSection: async (section: Seccion): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'Seccion', section);
  },
  deleteSection: async (processId: number, templateId: number, sectionId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Seccion/' + processId + '/' + templateId + '/' + sectionId);
  },
  deleteAllSectionsFromTemplate: async (processId: number, templateId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Seccion/' + processId + '/' + templateId);
  },
  changeSectionsOrder: async (sectionList: any): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'Seccion/Orden', sectionList);
  },
  // Fields
  getAllFieldsOfTypeList: async (workflowId: number, templateId: number, sectionId: number, fieldId: number): Promise<any> => {   
    return await axiosService.get<any>(BASE_PATH + 'Plantilla/Listas/' + workflowId + '/' + templateId + '/' + sectionId + '/' + fieldId);
  },
  getAllFieldTypes: async (): Promise<any> => {    
    return await axiosService.get<any>(BASE_PATH + 'TiposCampo/');
  },
  addField: async (field: Campo): Promise<any> => {
    return await axiosService.post(BASE_PATH + 'Campo', field);
  },
  editField: async (field: Campo): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'Campo', field);
  },
  deleteField: async (processId: number, templateId: number, sectionId:number, fieldId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Campo/' + processId + '/' + templateId + '/' + sectionId + '/' + fieldId);
  },
  deleteAllFieldsFromSection: async (processId: number, templateId: number, sectionId:number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Campo/' + processId + '/' + templateId + '/' + sectionId);
  },
  changeFieldsOrder: async (fieldList: Campo[]): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'Campo/Orden', fieldList);
  },
  // Template
  publishTemplate: async (tempTemplate: any): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'PublicarPlantilla', tempTemplate);
  },
  revert: async (tempTemplate: any): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'RevertirPlantilla/', tempTemplate);
  },  
  // List Values
  getListValues: async (workflowIdId: number, templateId: number, sectionId:number, fieldId: number, parentFieldId: number, parentValueId: number): Promise<any> => {   
    return await axiosService.get<any>(BASE_PATH + 'Plantilla/Lista/' + workflowIdId + '/' + templateId + '/' + sectionId + '/' + fieldId + '/' + parentFieldId + '/' + parentValueId, {});
  },
  getCurrentTemplateListValues: async (workflowIdId: number, templateId: number, sectionId:number, fieldId: number, parentFieldId: number, parentValueId: number): Promise<any> => {   
    return await axiosService.get<any>(BASE_PATH + 'PlantillaActual/Lista/' + workflowIdId + '/' + templateId + '/' + sectionId + '/' + fieldId + '/' + parentFieldId + '/' + parentValueId, {});
  },
  addListValue: async (listValue: ValorLista): Promise<any> => {   
    return await axiosService.post<any>(BASE_PATH + 'Plantilla/Lista', listValue);
  },
  deleteListValue: async (workflowId: number, templateId: number, sectionId: number, fieldId: number, valueId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Plantilla/Lista/' + workflowId + '/' + templateId + '/' + sectionId + '/' + fieldId + '/' + valueId);
  },
  deleteAllListValues: async (workflowId: number, templateId: number, sectionId: number, fieldId: number): Promise<any> => {
    return await axiosService.delete(BASE_PATH + 'Plantilla/Lista/' + workflowId + '/' + templateId + '/' + sectionId + '/' + fieldId);
  },
  setDefaultValue: async (value: ValorLista): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'Plantilla/Lista', value);
  },
  changeListValuesOrder: async (valuesList: ValorLista[]): Promise<any> => {
    return await axiosService.put(BASE_PATH + 'Plantilla/Lista/Valor/Orden', valuesList);
  }
};

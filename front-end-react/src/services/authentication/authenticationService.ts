import axiosService from "../axios/axiosService";

const BASE_PATH = "api/Autenticacion/";

export const authenticationService = {

  ping: async (): Promise<any> => {
      return axiosService.get<any>(BASE_PATH + 'Ping');
  },

  signIn: async(userName: string, password: string): Promise<any> => {    
      const data = {
        idUsuario: 0,
        nombreUsuario: userName,
        password: password,
        requiereCambioPassword: false
      }
      return await axiosService.post<any>(BASE_PATH + 'Login', data);
  }
};

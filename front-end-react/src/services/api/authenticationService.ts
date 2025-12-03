import axios from "axios";

const BASE_PATH = "api/Authentication/";

export const authenticationService = {

  ping: async (): Promise<any> => {
      return axios.get<any>(BASE_PATH + 'Ping');
  },

  signIn: async(userName: string, password: string): Promise<any> => {    
      const data = {
        userId: 0,
        userName: userName,
        password: password,
        requiresPasswordChange: false
      }
      return await axios.post<any>(BASE_PATH + 'SignIn', data);
  }
};

import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { IAuthResponse } from '../models/IAuthResponse';
import { IUserRegistration } from '../models/IUserRegistration';
import { IRegisterResponse } from '../models/IRegisterResponse';

interface IAuthContext {
  children: any;
  login: (username: string, password: string) => Promise<IAuthResponse>;
  register: (data: IUserRegistration) => Promise<IRegisterResponse>;
  logout: () => void;
  isLoggedIn: () => boolean;
  getHeaders: () => any;
}

const AuthContext = React.createContext<IAuthContext>(null as any);

export const AuthProvider = ({ children }: { children: any }) => {
  const [jwt, setJwt] = useState<string | null>(null);

  useEffect(() => {
    let accessToken = localStorage.getItem('accessToken');

    if (accessToken) {
      setJwt(accessToken);
    }
  }, []);

  const login = (username: string, password: string): Promise<IAuthResponse> => {
    return axios
      .post('/api/auth/login', {
        username,
        password,
      })
      .then((res) => {
        if (res.data.token) {
          localStorage.setItem('accessToken', res.data.token);
          setJwt(res.data.token);
        }

        return res.data;
      });
  };

  const register = (data: IUserRegistration): Promise<IRegisterResponse> => {
    return axios
      .post('/api/auth/register', data)
      .then((res) => {
        if (res.data.auth.token) {
          localStorage.setItem('accessToken', res.data.auth.token);
          setJwt(res.data.auth.token);
        }

        return res.data;
      });
  };

  const logout = () => {
    setJwt(null);
    localStorage.removeItem('accessToken');
  };

  const isLoggedIn = (): boolean => {
    return jwt !== null;
  };

  const getHeaders = () => {
    return {
      headers: {
        Authorization: 'Bearer ' + jwt,
      },
    };
  };

  return (
    <AuthContext.Provider
      value={{
        children,
        login,
        register,
        logout,
        isLoggedIn,
        getHeaders,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;

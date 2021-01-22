import React, { useEffect, useState } from 'react';
import axios, { AxiosResponse } from 'axios';
import IAuthResponse from '../models/IAuthResponse';
import IUserRegistration from '../models/IUserRegistration';
import IUser from '../models/IUser';

interface IAuthContext {
  /**
   * React nodes that are childs of the AuthContextProvider
   */
  children: any;

  /**
   * Attempts to log in with given username and password, and stores user login credentials 
   * @param username Username
   * @param password Password
   */
  login: (username: string, password: string) => Promise<IAuthResponse>;

  /**
   * Attempts to register a new user, and saves their login credentials if successful
   */
  register: (data: IUserRegistration) => Promise<IAuthResponse>;

  /**
   * Logs out current logged in user
   */
  logout: () => void;

  /**
   * Returns whether the client is currently logged in or not
   */
  isLoggedIn: () => boolean;

  /**
   * Returns the current user if they're logged in, or null otherwise
   */
  getUser: () => IUser | null;

  /**
   * Returns whether a user is part of a given role
   */
  isInRole: (role: string) => boolean;

  /**
   * Returns REST API request headers that need to be sent in order to authenticate
   */
  getHeaders: () => any;
}

const AuthContext = React.createContext<IAuthContext>(null as any);

export const AuthProvider = ({ children }: { children: any }) => {
  const [jwt, setJwt] = useState<string | null>(null);
  const [user, setUser] = useState<IUser | null>(null);
  const [roles, setRoles] = useState<string[]>([]);

  useEffect(() => {
    let data = localStorage.getItem('auth');

    if (data) {
      let jsonData = JSON.parse(data) as IAuthResponse;

      if (jsonData) {
        setJwt(jsonData.auth.token);
        setRoles(jsonData.auth.roles);
        setUser(jsonData.user);
      }
    }
  }, []);

  const login = (username: string, password: string): Promise<IAuthResponse> => {
    return axios
      .post('/api/auth/login', {
        username,
        password,
      })
      .then((res: AxiosResponse<IAuthResponse>) => {
        let data = res.data as IAuthResponse;
        console.log(data);

        if (data.user && data.auth) {
          localStorage.setItem('auth', JSON.stringify(data));

          setJwt(data.auth.token);
          setRoles(data.auth.roles);
          setUser(data.user);
        }

        return data;
      });
  };

  const register = (data: IUserRegistration): Promise<IAuthResponse> => {
    return axios
      .post('/api/auth/register', data)
      .then((res: AxiosResponse<IAuthResponse>) => {
        let data = res.data as IAuthResponse;
        console.log(data);

        if (data.auth.token) {
          localStorage.setItem('auth', JSON.stringify(data));

          setJwt(data.auth.token);
          setRoles(data.auth.roles);
          setUser(data.user);
        }

        return data;
      });
  };

  const logout = () => {
    setJwt(null);
    setUser(null);
    setRoles([]);

    localStorage.removeItem('auth');
  };

  const isLoggedIn = (): boolean => {
    return jwt !== null;
  };

  const getUser = (): IUser | null => {
    return user;
  }

  const isInRole = (role: string): boolean => {
    if (!user || !jwt || !roles) return false;
    return roles.includes(role);
  }

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
        getUser,
        isInRole,
        getHeaders,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;

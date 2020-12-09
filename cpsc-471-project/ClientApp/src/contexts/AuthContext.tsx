import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface IAuthContext {
  children: any;
  login: (username: string, password: string) => Promise<any>;
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

  const login = (username: string, password: string) => {
    return axios
      .post('/api/auth/login', {
        username,
        password,
      })
      .then((res) => {
        if (res.data.token) {
          localStorage.setItem('accessToken', res.data.token);
        }

        return res.data;
      });
  };

  const logout = () => {
    setJwt(null);
    localStorage.removeItem('accessToken');
  };

  const isLoggedIn = (): boolean => {
    return jwt === null;
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

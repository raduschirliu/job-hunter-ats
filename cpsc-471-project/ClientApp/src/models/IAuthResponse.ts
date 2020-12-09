import { IUser } from "./IUser";

export interface IAuthResponse {
  user: IUser;
  auth: {
    token: string;
    roles: string[];
    expiration: Date;
  };
};
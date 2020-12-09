import { IAuthResponse } from "./IAuthResponse";
import { IUser } from "./IUser";

export interface IRegisterResponse {
  user: IUser;
  auth: IAuthResponse;
};
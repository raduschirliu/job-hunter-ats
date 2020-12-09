import IUser from './IUser';

export default interface IAuthResponse {
  user: IUser;
  auth: {
    token: string;
    roles: string[];
    expiration: Date;
  };
}

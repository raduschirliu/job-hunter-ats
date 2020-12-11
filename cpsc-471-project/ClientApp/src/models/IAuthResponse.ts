import IUser from './IUser';

/**
 * Representation of the object returned when either logging in or registering a new user
 */
export default interface IAuthResponse {
  user: IUser;
  auth: {
    token: string;
    roles: string[];
    expiration: Date;
  };
}

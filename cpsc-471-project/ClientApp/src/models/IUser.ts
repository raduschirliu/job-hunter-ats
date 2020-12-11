/**
 * Representation of the UserDTO sent back from the API
 */
export default interface IUser {
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
};
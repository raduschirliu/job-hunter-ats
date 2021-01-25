/**
 * Data format needing to be sent to server to register a new user
 */
export default interface IUserRegistration {
  userName: string;
  password: string;
  firstName: string;
  lastName: string;
};
import IUser from "./IUser";

export default interface IResume {
  resumeId: number;
  name: string;
  candidate?: IUser;
};
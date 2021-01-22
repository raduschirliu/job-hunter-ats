import IUser from "./IUser";

/**
 * Representation of the ResumeDTO sent back from the API
 */
export default interface IResume {
  resumeId: number;
  name: string;
  candidate?: IUser;
  awards?: IAward[];
  certifications?: ICertification[];
  education?: IEducation[];
  experience?: IExperience[];
  projects?: IProject[];
  skills?: ISkill[];
};

// Various other weak resume entity DTOs sent back from the backend

export interface IAward {
  resumeId: number;
  order: number;
  name: string;
  dateReceived: Date;
  value: string;
};

export interface ICertification {
  resumeId: number;
  order: number;
  name: string;
  source: string;
};

export interface IEducation {
  resumeId: number;
  order: number;
  schoolName: string;
  major: string;
  startDate: Date;
  endDate?: Date;
};

export interface IExperience {
  resumeId: number;
  order: number;
  company: string;
  title: string;
  startDate: Date;
  endDate?: Date;
};

export interface IProject {
  resumeId: number;
  order: number;
  name: string;
  description: string;
  startDate?: Date;
  endDate?: Date;
};

export interface ISkill {
  resumeId: number;
  order: number;
  name: string;
  proficiency: string;
};
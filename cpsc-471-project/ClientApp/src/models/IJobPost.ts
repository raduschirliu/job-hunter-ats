/**
 * Representation of the JobPostDTO returned from the API
 */
export default interface IJobPost {
  jobPostId: number;
  companyId: number;
  name: string;
  description: string;
  salary: number;
  closingDate: Date;
};
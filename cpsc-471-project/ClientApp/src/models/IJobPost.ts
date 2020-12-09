export default interface IJobPost {
  jobPostId: number;
  companyId: number;
  name: string;
  description: string;
  salary: number;
  closingDate: Date;
};
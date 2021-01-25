/**
 * Representation of the CompanyDTO sent back from the API
 */
export default interface ICompany {
  companyId: number;
  adminId: string;
  description: string;
  industry: string;
  name: string;
  size: number;
};
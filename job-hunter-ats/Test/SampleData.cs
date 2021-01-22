#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

using job_hunter_ats.Authentication;
using job_hunter_ats.Models;


/*
To add sample data, simply add the following line at the top of the document
using job_hunter_ats.Test;

And then copy this code to the UserController.cs file (allongside the other APIs)

#if DEBUG
        // POST: api/Users/SampleData
        [HttpPost("PopulateDB")]
        public async Task<ActionResult<User>> PopulateDB()
        {
            SampleData.AddSampleData(_context);
            return Ok("Sample data has been added if there was no existing data");
        }
#endif

Then, to add the sample data, you simply have to make a POST request to https://localhost:5001/api/Users/PopulateDB using Postman
*/

namespace job_hunter_ats.Test
{
    public class SampleData
    {
        public static async Task AddSampleData(JobHunterDBContext _context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!_context.Users.Any())
            {
                await AddSampleUserData(userManager);
            }
            if (!_context.Companies.Any())
            {
                _context.Companies.AddRange(SampleCompanyData());
            }
            if (!_context.Resumes.Any())
            {
                _context.Resumes.AddRange(SampleResumeData());
            }
            if (!_context.JobPosts.Any())
            {
                _context.JobPosts.AddRange(SampleJobPostData());
            }
            if (!_context.Skills.Any())
            {
                _context.Skills.AddRange(SampleSkillData());
            }
            
            if (!_context.Certifications.Any())
            {
                _context.Certifications.AddRange(SampleCertificationData());
            }
            if (!_context.Education.Any())
            {
                _context.Education.AddRange(SampleEducationData());
            }
            if (!_context.Experiences.Any())
            {
                _context.Experiences.AddRange(SampleExperienceData());
            }
            if (!_context.Awards.Any())
            {
                _context.Awards.AddRange(SampleAwardData());
            }
            if(!_context.Projects.Any())
            {
                _context.Projects.AddRange(SampleProjectData());
            }

            if (!_context.Applications.Any())
            {
                _context.Applications.AddRange(SampleApplicationData());
            }
            if (!_context.Referrals.Any())
            {
                _context.Referrals.AddRange(SampleReferralData());
            }
            if (!_context.Recruiters.Any())
            {
                _context.Recruiters.AddRange(SampleRecruiterData());
            }

            if (!_context.Offers.Any())
            {
                _context.Offers.AddRange(SampleOfferData());
            }
            if (!_context.Interviews.Any())
            {
                _context.Interviews.AddRange(SampleInterviewData());
            }
            await _context.SaveChangesAsync();
        }

        public static async Task AddSampleUserData(UserManager<User> userManager)
        {
            User adminUser = new User()
            {
                Id = "admin-user",
                FirstName = "Bob",
                LastName = "Ross",
                UserName = "admin-user",
                Email = "bobross@12asgaetrfasfasf.com",
                PhoneNumber = "555-555-5555",
            };
            await userManager.CreateAsync(adminUser, "password");
            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);

            User recruiterUser1 = new User()
            {
                Id = "recruiter-1",
                FirstName = "Recruiter",
                LastName = "Person",
                UserName = "recruiter-1",
                Email = "recruiter@afjaasdidaoifmasfa.com",
                PhoneNumber = "555-555-5555",
            };
            await userManager.CreateAsync(recruiterUser1, "password");
            await userManager.AddToRoleAsync(recruiterUser1, UserRoles.Recruiter);

            User recruiterUser2 = new User()
            {
                Id = "recruiter-2",
                FirstName = "Recruiter2",
                LastName = "Person2",
                UserName = "recruiter-2",
                Email = "recruiter2@afjaasdidaoifmasfa.com",
                PhoneNumber = "555-555-5555",
            };
            await userManager.CreateAsync(recruiterUser2, "password");
            await userManager.AddToRoleAsync(recruiterUser2, UserRoles.Recruiter);

            User recruiterUser3 = new User()
            {
                Id = "recruiter-3",
                FirstName = "Recruiter3",
                LastName = "Person3",
                UserName = "recruiter-3",
                Email = "recruiter3@afjaasdidaoifmasfa.com",
                PhoneNumber = "555-555-5555",
            };
            await userManager.CreateAsync(recruiterUser3, "password");
            await userManager.AddToRoleAsync(recruiterUser3, UserRoles.Recruiter);

            User candidateUser1 = new User()
            {
                Id = "bob-smith",
                FirstName = "Bob",
                LastName = "Smith",
                UserName = "bob-smith",
                Email = "bobsmith12345@jgd098suyfvk23jbfjsdv.com",
                PhoneNumber = "555-555-5555",
            };
            await userManager.CreateAsync(candidateUser1, "password");
            await userManager.AddToRoleAsync(candidateUser1, UserRoles.Candidate);

            User candidateUser2 = new User()
            {
                Id = "caitlyn-brown",
                FirstName = "Caitlyn",
                LastName = "Brown",
                UserName = "caitlyn-brown",
                Email = "caitlynbrown1@jgd098suyfvk23jbfjsdv.com",
                PhoneNumber = "444-444-4444",
            };
            await userManager.CreateAsync(candidateUser2, "password");
            await userManager.AddToRoleAsync(candidateUser2, UserRoles.Candidate);

            User candidateUser3 = new User() {
                Id = "evan-johnson",
                FirstName = "Evan",
                LastName = "Johnson",
                UserName = "evan-johnson",
                Email = "evanjohnson@jgd098suyfvk23jbfjsdv.com",
                PhoneNumber = "333-333-3333",
            };
            await userManager.CreateAsync(candidateUser3, "password");
            await userManager.AddToRoleAsync(candidateUser3, UserRoles.Candidate);
        }
        public static List<Company> SampleCompanyData()
        {
            List<Company> returnedCompanies = new List<Company>();

            returnedCompanies.Add(new Company()
            {
                CompanyId = 1,
                Size = CompanySize.OneToTen,
                Name = "Test Company 1",
                Description = "Test Description 1",
                Industry = "Technology",
                AdminId = "admin-user"
            });

            returnedCompanies.Add(new Company()
            {
                CompanyId = 2,
                Size = CompanySize.ElevenToFifty,
                Name = "Test Company 2",
                Description = "Test Description 2",
                Industry = "Retail",
                AdminId = "admin-user",
            });

            returnedCompanies.Add(new Company()
            {
                CompanyId = 3,
                Size = CompanySize.FiftyOneToTwoFifty,
                Name = "Test Company 3",
                Description = "Test Description 3",
                Industry = "Manufacturing",
                AdminId = "admin-user"
            });

            return returnedCompanies;
        }
        public static List<Resume> SampleResumeData()
        {
            List<Resume> returnedResumes = new List<Resume>();

            Resume testResume1 = new Resume()
            {
                ResumeId = 1,
                Name = "Resume #1",
                CandidateId = "bob-smith"
            };

            returnedResumes.Add(testResume1);

            Resume testResume2 = new Resume()
            {
                ResumeId = 2,
                Name = "Resume #2",
                CandidateId = "bob-smith"
            };
            returnedResumes.Add(testResume2);

            Resume testResume3 = new Resume()
            {
                ResumeId = 3,
                Name = "Resume #3",
                CandidateId = "caitlyn-brown",
            };
            returnedResumes.Add(testResume3);

            return returnedResumes;
        }
        public static List<Skill> SampleSkillData()
        {
            List<Skill> returnedSkills = new List<Skill>();

            Skill testSkill1 = new Skill()
            {
                ResumeId = 1,
                Name = "Python",
                Proficiency = "Experienced",
                Order = 1,
            };
            returnedSkills.Add(testSkill1);

            Skill testSkill2 = new Skill()
            {
                ResumeId = 1,
                Name = "Java",
                Proficiency = "Strong",
                Order = 2,
            };
            returnedSkills.Add(testSkill2);

            Skill testSkill3 = new Skill()
            {
                ResumeId = 2,
                Name = "C#",
                Proficiency = "Experienced",
                Order = 1,
            };
            returnedSkills.Add(testSkill3);

            Skill testSkill4 = new Skill()
            {
                ResumeId = 3,
                Name = "Rust",
                Proficiency = "Fluent",
                Order = 1,
            };
            returnedSkills.Add(testSkill4);

            return returnedSkills;
        }
        public static List<Certification> SampleCertificationData()
        {
            List<Certification> returnedCertifications = new List<Certification>();

            Certification testCertification1 = new Certification()
            {
                ResumeId = 1,
                Name = "Python Fundamentals",
                Source = "Python 101 Studios",
                Order = 1,
            };
            returnedCertifications.Add(testCertification1);

            Certification testCertification2 = new Certification()
            {
                ResumeId = 1,
                Name = "First Aid",
                Source = "First Aid Training Corp.",
                Order = 2,
            };
            returnedCertifications.Add(testCertification2);

            Certification testCertification3 = new Certification()
            {
                ResumeId = 3,
                Name = "Food Safety",
                Source = "Food Safety Council",
                Order = 1,
            };
            returnedCertifications.Add(testCertification3);

            return returnedCertifications;
        }

        public static List<Education> SampleEducationData()
        {
            DateTime d = new DateTime(1, 1, 1);
            List<Education> returnedEducations = new List<Education>();

            Education testEducation = new Education()
            {
                ResumeId = 1,
                SchoolName = "University of Calgary",
                StartDate = d,
                EndDate = d,
                Major = "Software Engineering",
                Order = 1,

            };
            returnedEducations.Add(testEducation);

            Education testEducation1 = new Education()
            {
                ResumeId = 2,
                SchoolName = "University of Calgary",
                StartDate = d,
                EndDate = d,
                Major = "Computer Science",
                Order = 1,
            };
            returnedEducations.Add(testEducation1);

            return returnedEducations;
        }
        
        public static List<Experience> SampleExperienceData()
        {
            List<Experience> returnedExperience = new List<Experience>();

            Experience testExperience1 = new Experience()
            {
                ResumeId = 1,
                Company = "Python Consulting",
                StartDate = new DateTime(2019, 10, 1),
                EndDate = new DateTime(2020, 10, 1),
                Title = "Python Developer",
                Order = 1,
            };
            returnedExperience.Add(testExperience1);

            Experience testExperience2 = new Experience()
            {
                ResumeId = 1,
                Company = "Java Consulting",
                StartDate = new DateTime(2018, 10, 1),
                EndDate = new DateTime(2019, 9, 30),
                Title = "Software Architect",
                Order = 2,
            };
            returnedExperience.Add(testExperience2);

            return returnedExperience;
        }

        public static List<Award> SampleAwardData()
        {
            List<Award> returnedAwards = new List<Award>();

            Award testAward = new Award()
            {
                ResumeId = 1,
                Name = "Software Developer of the Year, Python Consulting",
                DateReceived = new DateTime(2019, 12, 1),
                Value = "nominal",
                Order = 1,
            };
            returnedAwards.Add(testAward);

            Award testAward1 = new Award()
            {
                ResumeId = 1,
                Name = "1st Place, Algorithm Development Competition",
                DateReceived = new DateTime(2018, 12, 1),
                Value = "$750",
                Order = 2,
            };
            returnedAwards.Add(testAward1);

            return returnedAwards;
        }

        public static List<Project> SampleProjectData()
        {
            List<Project> returnedProjects = new List<Project>();

            Project testProject1 = new Project()
            {
                ResumeId = 1,
                Name = "Learning Management Tool",
                Description = "Helps manage learning for staff of large organizations",
                StartDate = new DateTime(2019, 10, 1),
                EndDate = new DateTime(2020, 10, 1),
                Order = 1,
            };
            returnedProjects.Add(testProject1);

            Project testProject2 = new Project()
            {
                ResumeId = 1,
                Name = "Network Mapping Tool",
                StartDate = new DateTime(2018, 10, 1),
                EndDate = new DateTime(2019, 9, 30),
                Order = 2,
            };
            returnedProjects.Add(testProject2);

            return returnedProjects;
        }

        public static List<JobPost> SampleJobPostData()
        {
            List<JobPost> returnedPosts = new List<JobPost>();

            JobPost testPost1 = new JobPost()
            {
                JobPostId = 1,
                CompanyId = 1,
                Name = "Software Developer",
                Description = "Develop software.",
                Salary = 100000,
                ClosingDate = new DateTime(2020, 07, 10)
            };
            returnedPosts.Add(testPost1);

            JobPost testPost2 = new JobPost()
            {
                JobPostId = 2,
                CompanyId = 2,
                Name = "Vexillologist",
                Description = "Makes flags.",
                Salary = 45000,
                ClosingDate = DateTime.Now.AddDays(10)
            };
            returnedPosts.Add(testPost2);

            return returnedPosts;
        }

        public static List<Application> SampleApplicationData()
        {
            return new List<Application>()
            {
                new Application()
                {
                    ApplicationId = 1,
                    JobId = 1,
                    DateSubmitted = new DateTime(2008, 5, 1, 8, 30, 52),
                    Status = StatusEnum.Accepted,
                    CoverLetter = "pls give job xo",
                    ResumeId = 1
                },
                new Application()
                {
                    ApplicationId = 2,
                    JobId = 1,
                    DateSubmitted = new DateTime(2018, 5, 1, 8, 30, 52),
                    Status = StatusEnum.Sent,
                    CoverLetter = "hello i want job",
                    ResumeId = 2
                },
                new Application()
                {
                    ApplicationId = 3,
                    JobId = 2,
                    DateSubmitted = DateTime.Now,
                    Status = StatusEnum.Rejected,
                    CoverLetter = "hi. please hire <3",
                    ResumeId = 3
                },
            };
        }
        public static List<Referral> SampleReferralData()
        {
            List<Referral> returnedRefs = new List<Referral>();

            Referral testRef1 = new Referral()
            {
                ApplicationId = 1,
                Application = null,
                ReferralId = 0,
                Name = "Jane Doe",
                Email = "jane@gmail.com",
                Position = "HR Manager",
                Company = "Google",
                Phone = "555-123-5555"
            };
            returnedRefs.Add(testRef1);

            Referral testRef2 = new Referral()
            {
                ApplicationId = 1,
                Application = null,
                ReferralId = 1,
                Name = "Dua Lipa",
                Email = "dua@lipa.com",
                Position = "Pop Icon",
                Company = "Billboard",
                Phone = "555-123-1234"
            };
            returnedRefs.Add(testRef2);

            return returnedRefs;
        }

        public static List<Recruiter> SampleRecruiterData()
        {
            return new List<Recruiter>()
            {
                new Recruiter()
                {
                    UserId = "recruiter-1",
                    CompanyId = 1,
                },
                new Recruiter()
                {
                    UserId = "recruiter-2",
                    CompanyId = 1,
                },
                new Recruiter()
                {
                    UserId = "recruiter-3",
                    CompanyId = 2,
                }
            };
        }
        
        public static List<Offer> SampleOfferData()
        {
            List<Offer> returnedOffers = new List<Offer>();

            Offer testOffer1 = new Offer()
            {
                ApplicationId = 1,
                OfferId = 1,
                AcceptanceEndDate = new DateTime(2020, 12, 1, 8, 30, 52),
                Text = "We are extending an offer as a contract salesperson for $20.25/hr."
            };
            returnedOffers.Add(testOffer1);

            Offer testOffer2 = new Offer()
            {
                ApplicationId = 1,
                OfferId = 2,
                AcceptanceEndDate = new DateTime(2008, 5, 1, 8, 30, 52),
                Text = "We are extending an offer as a lifeguard."
            };
            returnedOffers.Add(testOffer2);

            return returnedOffers;
        }

        public static List<Interview> SampleInterviewData()
        {
            return new List<Interview>()
            {
                new Interview()
                {
                    RecruiterId = "recruiter-1",
                    ApplicationId = 1,
                    Date = DateTime.Now
                },
                new Interview()
                {
                    RecruiterId = "recruiter-1",
                    ApplicationId = 2,
                    Date = new DateTime(2008, 5, 1, 8, 30, 52)
                },
                new Interview()
                {
                    RecruiterId = "recruiter-3",
                    ApplicationId = 3,
                    Date = DateTime.Now
                }
            };
        }
    }
}
#endif
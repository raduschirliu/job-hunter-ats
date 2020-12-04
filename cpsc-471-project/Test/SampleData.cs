#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

using cpsc_471_project.Authentication;
using cpsc_471_project.Models;


/*
To add sample data, simply add the following line at the top of the document
using cpsc_471_project.Test;

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

namespace cpsc_471_project.Test
{
    public class SampleData
    {
        public static async Task AddSampleData(JobHunterDBContext _context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!_context.Users.Any())
            {
                await AddSampleUserData(userManager);
            }
            if ( !_context.Companies.Any() )
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
            if (!_context.Applications.Any())
            {
                _context.Applications.AddRange(SampleApplicationData());
            }
            if (!_context.Recruiters.Any())
            {
                _context.Recruiters.AddRange(SampleRecruiterData());
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
                UserName = "bob-ross",
                Email = "bobross@12asgaetrfasfasf.com",
                PhoneNumber = "555-555-5555",
            };
            await userManager.CreateAsync(adminUser, "password");
            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);

            User recruiterUser = new User()
            {
                Id = "recruiter-user-1",
                FirstName = "Recruiter",
                LastName = "Person",
                UserName = "recruiter-1",
                Email = "recruiter@afjaasdidaoifmasfa.com",
                PhoneNumber = "555-555-5555",
            };
            await userManager.CreateAsync(recruiterUser, "password");
            await userManager.AddToRoleAsync(recruiterUser, UserRoles.Recruiter);

            await userManager.CreateAsync(new User()
            {
                Id = "user-1",
                FirstName = "Bob",
                LastName = "Smith",
                UserName = "bob-smith",
                Email = "bobsmith12345@jgd098suyfvk23jbfjsdv.com",
                PhoneNumber = "555-555-5555",
            }, "password");

            await userManager.CreateAsync(new User()
            {
                Id = "user-2",
                FirstName = "Caitlyn",
                LastName = "Brown",
                UserName = "caitlyn-brown",
                Email = "caitlynbrown1@jgd098suyfvk23jbfjsdv.com",
                PhoneNumber = "444-444-4444",
            }, "password");

            await userManager.CreateAsync(new User()
            {
                Id = "user-3",
                FirstName = "Evan",
                LastName = "Johnson",
                UserName = "evan-johnson",
                Email = "evanjohnson@jgd098suyfvk23jbfjsdv.com",
                PhoneNumber = "333-333-3333",
            }, "password");
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
            List<Resume> resumes = new List<Resume>();

            resumes.Add(new Resume()
            {
                ResumeId = 1,
                Name = "Resume #1",
                CandidateId = "user-1"
            });

            resumes.Add(new Resume()
            {
                ResumeId = 2,
                Name = "Resume #2",
                CandidateId = "user-1"
            });

            resumes.Add(new Resume()
            {
                ResumeId = 3,
                Name = "Gr8 resume",
                CandidateId = "user-3"
            });

            return resumes;
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
                ClosingDate = DateTime.Now,
                RecruiterId = "user-1"
            };
            returnedPosts.Add(testPost1);

            JobPost testPost2 = new JobPost()
            {
                JobPostId = 2,
                CompanyId = 2,
                Name = "Vexillologist",
                Description = "Makes flags.",
                Salary = 45000,
                ClosingDate = DateTime.Now,
                RecruiterId = "user-1"
            };
            returnedPosts.Add(testPost2);

            return returnedPosts;
        }

        public static List<Application> SampleApplicationData()
        {
            List<Application> returnedApps = new List<Application>();

            Application testApp1 = new Application()
            {
                ApplicationId = 1,
                JobId = 1,
                DateSubmitted = new DateTime(2008, 5, 1, 8, 30, 52),
                Status = StatusEnum.Accepted,
                CoverLetter = "pls give job xo",
                ResumeId = 2
            };
            returnedApps.Add(testApp1);
            return returnedApps;
        }

        public static List<Recruiter> SampleRecruiterData()
        {
            return new List<Recruiter>()
            {
                new Recruiter()
                {
                    UserId = "recruiter-user-1",
                    CompanyId = 1,
                }
            };
        }
    }
}
#endif
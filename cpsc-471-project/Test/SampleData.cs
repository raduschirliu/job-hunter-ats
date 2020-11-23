#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cpsc_471_project.Models;
using Microsoft.EntityFrameworkCore.Internal;


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
        public static void AddSampleData(JobHunterDBContext _context)
        {
            if(!_context.Resume.Any())
            {
                _context.Resume.AddRange(SampleResumeData());
            }
            if( !_context.Users.Any() )
            {
                _context.Users.AddRange(SampleUserData());
            }
            if ( !_context.Company.Any() )
            {
                _context.Company.AddRange(SampleCompanyData());
            }
            if (!_context.Skill.Any())
            {
                _context.Skill.AddRange(SampleSkillData());
            }
            
            if (!_context.Certification.Any())
            {
                _context.Certification.AddRange(SampleCertificationData());
            }
            if (!_context.Education.Any())
            {
                _context.Education.AddRange(SampleEducationData());
            }
            if (!_context.Experience.Any())
            {
                _context.Experience.AddRange(SampleExperienceData());
            }
            if (!_context.Award.Any())
            {
                _context.Award.AddRange(SampleAwardData());
            }
            
            _context.SaveChanges();
        }

        public static List<User> SampleUserData()
        {
            List<User> returnedUsers = new List<User>();

            User testUser1 = new User()
            {
                UserId = 1,
                Role = UserRole.Admin,
                FirstName = "Bob",
                LastName = "Smith",
                Email = "bobsmith12345@jgd098suyfvk23jbfjsdv.com",
                Phone = "555-555-5555",
            };
            returnedUsers.Add(testUser1);

            User testUser2 = new User()
            {
                UserId = 2,
                Role = UserRole.Recruiter,
                FirstName = "Caitlyn",
                LastName = "Brown",
                Email = "caitlynbrown1@jgd098suyfvk23jbfjsdv.com",
                Phone = "444-444-4444",
            };
            returnedUsers.Add(testUser2);

            User testUser3 = new User()
            {
                UserId = 3,
                Role = UserRole.Candidate,
                FirstName = "Evan",
                LastName = "Johnson",
                Email = "evanjohnson@jgd098suyfvk23jbfjsdv.com",
                Phone = "333-333-3333",
            };
            returnedUsers.Add(testUser3);

            return returnedUsers;
        }
        public static List<Company> SampleCompanyData()
        {
            List<Company> returnedCompanies = new List<Company>();

            Company testCompany1 = new Company()
            {
                CompanyId = 1,
                Size = CompanySize.OneToTen,
                Name = "Test Company 1",
                Description = "Test Description 1",
                Industry = "Technology",
                UserId = 1
            };
            returnedCompanies.Add(testCompany1);

            Company testCompany2 = new Company()
            {
                CompanyId = 2,
                Size = CompanySize.ElevenToFifty,
                Name = "Test Company 2",
                Description = "Test Description 2",
                Industry = "Retail",
                UserId = 1
            };
            returnedCompanies.Add(testCompany2);

            Company testCompany3 = new Company()
            {
                CompanyId = 3,
                Size = CompanySize.FiftyOneToTwoFifty,
                Name = "Test Company 3",
                Description = "Test Description 3",
                Industry = "Manufacturing",
                UserId = 1
            };
            returnedCompanies.Add(testCompany3);

            return returnedCompanies;
        }
        public static List<Resume> SampleResumeData()
        {
            List<Resume> returnedResumes = new List<Resume>();

            Resume testResume1 = new Resume()
            {
                ResumeId = 1,
                CandidateId = 1,
                Name = "resume1"

            };
            returnedResumes.Add(testResume1);

            Resume testResume2 = new Resume()
            {
                ResumeId = 2,
                CandidateId = 2,
                Name = "resume2"

            };
            returnedResumes.Add(testResume2);

            Resume testResume3 = new Resume()
            {
                ResumeId = 3,
                CandidateId = 3,
                Name = "resume3"

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
                Name = "coding",
                Proficiency = "weak",
            };
            returnedSkills.Add(testSkill1);

            Skill testSkill2 = new Skill()
            {
                ResumeId = 2,
                Name = "codingggg",
                Proficiency = "weakkkk",
            };
            returnedSkills.Add(testSkill2);

            Skill testSkill3 = new Skill()
            {
                ResumeId = 3,
                Name = "cccccoding",
                Proficiency = "wwwwweak",
            };
            returnedSkills.Add(testSkill3);

            return returnedSkills;
        }
        public static List<Certification> SampleCertificationData()
        {
            List<Certification> returnedCertifications = new List<Certification>();

            Certification testCertification1 = new Certification()
            {
                ResumeId = 1,
                Name = "coding",
                Source = "weak",
            };
            returnedCertifications.Add(testCertification1);

            Certification testCertification2 = new Certification()
            {
                ResumeId = 2,
                Name = "codingggg",
                Source = "weakkkk",
            };
            returnedCertifications.Add(testCertification2);

            Certification testCertification3 = new Certification()
            {
                ResumeId = 3,
                Name = "cccccoding",
                Source = "wwwwweak",
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
                Name = "coding",
                StartDate = d,
                EndDate = d,
                Major = "watermelon"
            };
            returnedEducations.Add(testEducation);

            Education testEducation1 = new Education()
            {
                ResumeId = 2,
                Name = "codingggggg",
                StartDate = d,
                EndDate = d,
                Major = "waterdddddmelon"
            };
            returnedEducations.Add(testEducation1);

            return returnedEducations;
        }
        public static List<Experience> SampleExperienceData()
        {
            DateTime d = new DateTime(1, 1, 1);
            List<Experience> returnedExperience = new List<Experience>();

            Experience testExperience = new Experience()
            {
                ResumeId = 1,
                Company = "coding",
                StartDate = d,
                EndDate = d,
                Title = "watermelon"
            };
            returnedExperience.Add(testExperience);

            Experience testExperience1 = new Experience()
            {
                ResumeId = 2,
                Company = "codddddddding",
                StartDate = d,
                EndDate = d,
                Title = "waterrrrrrmelon"
            };
            returnedExperience.Add(testExperience1);

            return returnedExperience;
        }
        public static List<Award> SampleAwardData()
        {
            DateTime d = new DateTime(1, 1, 1);
            List<Award> returnedAwards = new List<Award>();

            Award testAward = new Award()
            {
                ResumeId = 1,
                Name = "coding",
                DateReceived = d,
                value = "watermelon"
            };
            returnedAwards.Add(testAward);

            Award testAward1 = new Award()
            {
                ResumeId = 2,
                Name = "codiiiing",
                DateReceived = d,
                value = "watermmmmmelon"
            };
            returnedAwards.Add(testAward1);

            return returnedAwards;
        }

    }
}
#endif
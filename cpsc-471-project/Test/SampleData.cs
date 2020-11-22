#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cpsc_471_project.Models;
using Microsoft.AspNetCore.Identity;
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
        public static async Task AddSampleData(JobHunterDBContext _context, UserManager<User> userManager)
        {
            if (!_context.Users.Any())
            {
                await AddSampleUserData(userManager);
            }

            if (!_context.Company.Any())
            {
                await _context.Company.AddRangeAsync(SampleCompanyData());
            }

            await _context.SaveChangesAsync();
        }

        public static async Task AddSampleUserData(UserManager<User> userManager)
        {
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
                AdminId = "user-1"
            });

            returnedCompanies.Add(new Company()
            {
                CompanyId = 2,
                Size = CompanySize.ElevenToFifty,
                Name = "Test Company 2",
                Description = "Test Description 2",
                Industry = "Retail",
                AdminId = "user-1",
            });

            returnedCompanies.Add(new Company()
            {
                CompanyId = 3,
                Size = CompanySize.FiftyOneToTwoFifty,
                Name = "Test Company 3",
                Description = "Test Description 3",
                Industry = "Manufacturing",
                AdminId = "user-3"
            });

            return returnedCompanies;
        }
    }
}
#endif
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using job_hunter_ats.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using job_hunter_ats.Authentication;

namespace job_hunter_ats
{ 
    public class Startup
    {
        public const string DBLocation = "Data Source=..\\JobHunterDB.sqlite";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<JobHunterDBContext>(opt => opt.UseSqlite(DBLocation));
            services.AddControllers();

            // For Identity
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<JobHunterDBContext>()
                .AddDefaultTokenProviders();

            // Adding Authentication and JWT Bearer
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });

            // Configure password policy to be more easy-going
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });

            // Configure CORS
            services.AddCors(options => options.AddPolicy("ReactPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("ReactPolicy");

            // Create roles if they do not exist
            CreateRoleIfNotExists(roleManager, UserRoles.Admin);
            CreateRoleIfNotExists(roleManager, UserRoles.Recruiter);
            CreateRoleIfNotExists(roleManager, UserRoles.Candidate);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Setup React SPA
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        public async void CreateRoleIfNotExists(RoleManager<IdentityRole> roleManager, string name)
        {
            if (await roleManager.RoleExistsAsync(name))
            {
                return;
            }

            await roleManager.CreateAsync(new IdentityRole()
            {
                Name = name
            });
        }
    }
}

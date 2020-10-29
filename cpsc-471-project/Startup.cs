using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using cpsc_471_project.Models;

namespace cpsc_471_project
{ 
    public class Startup
    {
        public const string DBLocation = "Data Source=..\\..\\JobHunterDB.sqlite";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<JobHunterDBContext>(opt => opt.UseSqlite(DBLocation));
            services.AddControllers();
            /*
#if DEBUG
            Trying to figure out how to add sample data here
            var serviceEnumerator = services.GetEnumerator();
            serviceEnumerator.MoveNext();
            while( serviceEnumerator != null)
            {
                if (serviceEnumerator.Current.ServiceType.Name.Contains( "JobHunterDBContext"))
                {
                    var userSampleData = User.SampleData();
                    JobHunterDBContext database = (JobHunterDBContext) serviceEnumerator.Current;
                    break;
                }
                serviceEnumerator.MoveNext();
            }
#endif
            */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

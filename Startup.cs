using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace butterystrava
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();

            //services.AddDistributedMemoryCache(); // is for cookie?
            services.AddSession(options =>
            {
                options.Cookie.Name = ".buttery";
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // i dont like this one bit.
            // tried user-secrets, but that's not encrypted
            // tried finding other methods but it seems env vars are the best cross platform method
            // not tied to "azure keys"
            var settings = new Strava.Settings {
                ClientId = Environment.GetEnvironmentVariable("STRAVA_CLIENT_ID"),
                ClientSecret = Environment.GetEnvironmentVariable("STRAVA_CLIENT_SECRET") 
            };
            services.AddSingleton(typeof(Strava.Settings), settings);

            var client = new Strava.Client(settings);
            services.AddSingleton(typeof(Strava.Client), client);

            //services.AddDbContext<Models.ButteryContext>(options => options.UseSqlite(connectionstring));
            services.AddDbContext<Models.ButteryContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else {
                app.UsePathBase(Configuration["pathBase"]);
                app.UseStaticFiles();
            }

            // figure out a better way to handle this for deployment. for now it's behind nginx and only ports 80/443 are open
            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name:"default", pattern:"{controller=Home}/{action=Index}");
            });
        }
    }
}

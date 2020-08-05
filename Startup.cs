using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;

using butterystrava.Buttery;
using Microsoft.AspNetCore.Authentication.Cookies;

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
            services.AddDistributedMemoryCache(); 

            /*services.AddSession(options =>
            {
                options.Cookie.Name = ".buttery";
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });*/

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

            services.AddDbContext<ButteryContext>();
            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<ButteryContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(o => { });

            // Add application services.
            // https://github.com/dotnet/aspnetcore/tree/master/src/Identity/samples/IdentitySample.Mvc
            services.AddTransient<butterystrava.Services.IEmailSender, butterystrava.Services.Messages>();
            //services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            //app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            } else {
                var path = Configuration["pathBase"];
                if (!string.IsNullOrEmpty(path))
                    app.UsePathBase(path);

            }

            // figure out a better way to handle this for deployment. for now it's behind nginx and only ports 80/443 are open
            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}

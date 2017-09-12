using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Airgap.Entity;
using Microsoft.EntityFrameworkCore;
using Airgap.Constant;
using Airgap.Service;
using Airgap.Service.Helper;

namespace Airgap.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings");
            services.Configure<AppSetting>(appSettings);
            // Add framework services.
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            //For all services, register here
            services.AddTransient<ITimerTypeService, TimerTypeService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAccountTokenService, AccountTokenService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IHelperService, HelperService>();
            services.AddTransient<IPasswordHistoryService, PasswordHistoryService>();
            services.AddTransient<IAccountApplianceService, AccountApplianceService>();
            services.AddTransient<IStateService, StateService>();
            services.AddTransient<IApplianceService, ApplianceService>();
            services.AddTransient<ITimerScheduleService, TimerScheduleService>();
            services.AddTransient<INotificationPreferenceService, NotificationPreferenceService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IStripeService, StripeService>();
            // Add framework services.
            services.AddAuthorization();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.CookieName = ".FileSystem";
            });
            services.AddMvc();
            services.AddMvc()
             .AddJsonOptions(setup =>
             {
                 setup.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

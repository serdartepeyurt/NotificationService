namespace NotificationService
{
    using Hangfire;
    using Hangfire.PostgreSql;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.NodeServices;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NotificationService.Core.Definitions;
    using NotificationService.Core.Service.Abstract;
    using NotificationService.Services;
    using System.Collections.Generic;
    using System.Linq;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            var queueConnStr = Configuration.GetValue<string>("QueueConnString");

            // Add framework services.
            services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(queueConnStr);
            });

            services.AddNodeServices();

            var apnsServices = new List<ApnsDefinition>();
            var fcmServices = new List<FcmDefinition>();
            var smtpServices = new List<SmtpDefinition>();

            var apnsServicesSection = Configuration.GetSection("ApnsDefinitions").GetChildren();
            if(apnsServicesSection != null)
            {
                apnsServices.AddRange(apnsServicesSection
                    .ToList()
                    .Select(x =>
                    new ApnsDefinition
                    {
                        ProjectIdentifier = x.GetValue<string>("ProjectIdentifier"),
                        ApnsCertPass = x.GetValue<string>("ApnsCertPass"),
                        ApnsCertPath = x.GetValue<string>("ApnsCertPath")
                    }));
            }

            var fcmServicesSection = Configuration.GetSection("FcmDefinitions").GetChildren();
            if (fcmServicesSection != null)
            {
                fcmServices.AddRange(fcmServicesSection
                    .ToList()
                    .Select(x =>
                    new FcmDefinition
                    {
                        ProjectIdentifier = x.GetValue<string>("ProjectIdentifier"),
                        FcmSenderId = x.GetValue<string>("FcmSenderId"),
                        FcmServerKey = x.GetValue<string>("FcmServerKey")
                    }));
            }

            var smtpServicesSection = Configuration.GetSection("SmtpDefinitions").GetChildren();
            if (smtpServicesSection != null)
            {
                smtpServices.AddRange(smtpServicesSection
                    .ToList()
                    .Select(x =>
                    new SmtpDefinition
                    {
                        ProjectIdentifier = x.GetValue<string>("ProjectIdentifier"),
                        EmailAddress = x.GetValue<string>("EmailAddress"),
                        SenderName = x.GetValue<string>("SenderName"),
                        Password = x.GetValue<string>("Password"),
                        SmtpServer = x.GetValue<string>("SmtpServer"),
                        Port = x.GetValue<int>("Port"),
                        UseSSL = x.GetValue<bool>("UseSSL")
                    }));
            }

            foreach (var def in apnsServices)
            {
                services.AddSingleton<INotificationService, ApnsService>((provider) =>
                {
                    return new ApnsService(def);
                });
            }

            foreach (var def in fcmServices)
            {
                services.AddSingleton<INotificationService, FcmService>((provider) =>
                {
                    return new FcmService(def);
                });
            }

            foreach (var def in smtpServices)
            {
                services.AddSingleton<INotificationService, SmtpEmailService>((provider) =>
                {
                    var nodeService = provider.GetService<INodeServices>();
                    return new SmtpEmailService(def, nodeService);
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}

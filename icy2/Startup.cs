using icy2.idsvr;
using icy2.idsvr.Endpoints;
using icy2.idsvr.ResponseHandling;
using icy2.idsvr.ResponseHandling.Interface;
using icy2.idsvr.Utility;
using icy2.idsvr.Validation;
using icy2.idsvr.Validation.Interface;
using Kiwi.Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace icy2
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
            //Kiwi Logging
            var loggerName = Configuration.GetSection("AppSettings:NLogLoggerName").Value;
            services.AddKiwiLogger(loggerName);

            //get certificate from a path
            var cert = new X509Certificate2("oauthcert.pfx", "1d3ntity$r", X509KeyStorageFlags.MachineKeySet);

            services.AddIdentityServer()
                .AddSigningCredential(cert) 
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddProfileService<idsvr.Service.MyProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddEndpoint<CustomExtendEndpoint>("extend", new Microsoft.AspNetCore.Http.PathString("/connect/extend"));

            services.AddMvc();
            services.AddScoped<IInternalClientValidator, InternalServiceClientValidator>();
            services.AddScoped<IGrantTypeValidator, InternalServiceGrantTypeValidator>();
            services.AddScoped<IAdditonalAudienceValidator, AdditonalAudienceValidator>();
            services.AddScoped<IJWTTokenValidator, JWTTokenValidator>();
            services.AddScoped<IExtendedTokenGenerator, ExtendedTokenGenerator>();
            ServiceProviderFactory.ServiceProvider = services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
                    ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Add Logging
            loggerFactory.UseKiwiLogger("Configs/nlog.config");                       

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

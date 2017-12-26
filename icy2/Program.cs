using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace icy2
{
    public class Program
    {
        public static void Main(string[] args)
        {         
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
           return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("Configs/appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"Configs/appsettings.{env.EnvironmentName}.json", optional: true);
                })
                .UseStartup<Startup>()                
                .Build();
        }
    }
}

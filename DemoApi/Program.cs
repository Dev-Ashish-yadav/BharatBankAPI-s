using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApi
{
    public class Program
    {
        public static string DgftBaseUrl = string.Empty;
        public static string Api_Key_data = string.Empty;
        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            IConfiguration config = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .Build();

            DgftBaseUrl = config["DGFTBaseUrl"];
            
            Api_Key_data = config["x-api-key"];
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GitHookReciever
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false)
                                                   .Build();
            CreateHostBuilder(args,int.Parse(config.GetSection("ListenPort").Value)).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, int listenPort) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseKestrel(options => options.ListenAnyIP(listenPort));
                });
    }
}

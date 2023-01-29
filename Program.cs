// See https://aka.ms/new-console-template for more information

using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weasel.Core;

namespace MartenDemo
{
    public static class Program
    {
        public static async  Task Main(string[] args)
        {
            IHost host = Create();
            await host.Services.GetRequiredService<IMartenService>().DoWorkAsync();
            
            Console.WriteLine("Hello, World!");
        }

        private static IHost Create()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((host, services) =>
                {
                    services.AddTransient<IMartenService, MartenService>();
                    
                    // This is the absolute, simplest way to integrate Marten into your
                    // .Net Core application with Marten's default configuration
                    services.AddMarten(options =>
                    {
                        // Establish the connection string to your Marten database
                        options.Connection("Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=postgres;");
                        options.AutoCreateSchemaObjects = AutoCreate.All;
                        
                        options.Schema.Include(new MyRegistry());
                        
                    });
                })
                .Build();
        }
    }
}
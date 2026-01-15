using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace P6_Hotel;

class Program
{
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args).
            ConfigureServices((context, services) =>
            {
                services.AddSingleton<HotelService>();
                services.AddSingleton<MenuService>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();
        
        var app = host.Services.GetRequiredService<MenuService>();
        app.ShowMenu();
    }
}
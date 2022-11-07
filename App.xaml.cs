using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyEmmControl.Communication;
using MyEmmControl.Views;
using MyEmmControl.ViewModes;

namespace MyEmmControl
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static IHost Host { get; private set; }

        public App()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddDebug();
                    //logging.AddNLog();
                })
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Add Services
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewMode>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await Host.StartAsync();
            var window = Host.Services.GetRequiredService<MainWindow>();
            window.DataContext = Host.Services.GetRequiredService<MainWindowViewMode>();
            window.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await Host.StopAsync();
            base.OnExit(e);
        }
    }
}

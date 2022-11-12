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
using System.Windows.Threading;

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
            services.AddSingleton<MainViewMode>();

            services.AddTransient<SelectCommunicationMode_Window>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            //UI线程未捕获异常处理事件
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            await Host.StartAsync();
            var window = Host.Services.GetRequiredService<MainWindow>();
            window.DataContext = Host.Services.GetRequiredService<MainViewMode>();
            window.Show();
            base.OnStartup(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await Host.StopAsync();
            base.OnExit(e);
        }
    }
}

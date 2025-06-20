using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyEmmControl.ViewModes;
using MyEmmControl.Views;

namespace MyEmmControl
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        private readonly ILogger<App> _logger;

        /// <summary>
        /// 配置文件目录
        /// </summary>
        private const string SETTINGSFILEPATH = @"Configs\Settings.json";

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .ConfigureHostConfiguration(cfgBulider =>
                {
                    cfgBulider.AddJsonFile(SETTINGSFILEPATH);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    //logging.AddDebug();
                    logging.AddFile();
                })
                .Build();

            _logger = _host.Services.GetRequiredService<ILogger<App>>();
        }

        /// <summary>
        /// Add Services
        /// </summary>
        /// <param name="services"> </param>
        private void ConfigureServices(IServiceCollection services)
        {
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

            await _host.StartAsync();

            _logger.LogInformation("OnStartUpCompleted");
            _host.Services.GetRequiredService<MainViewMode>();
            base.OnStartup(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.ToString());
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger.LogError(e.ToString());
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.ToString());
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            base.OnExit(e);
        }
    }
}

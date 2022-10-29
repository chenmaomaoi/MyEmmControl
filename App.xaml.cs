using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyEmmControl.Communication;

namespace MyEmmControl
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MyXbox xbox = new MyXbox();


            new MainWindow().Show();
            //new MyBLE_ConnectDeviceAndSettingWindow(new MyBLE()).ShowDialog();
        }
    }
}

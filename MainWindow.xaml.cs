using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MyEmmControl.Communication;
using SharpDX.Text;
using Windows.Devices.Enumeration;

namespace MyEmmControl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string prefix = null;

        public MainWindow()
        {
            InitializeComponent();

            var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ICommunication))))
                    .ToArray();
            foreach (var v in types)
            {
                cbx_CommunicationType.Items.Add(v.Name);
                if (string.IsNullOrEmpty(prefix))
                {
                    prefix = v.Namespace + '.';
                }
            }
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            string typeName = cbx_CommunicationType.SelectedItem.ToString();
            Type type = Type.GetType(prefix + typeName);
            ICommunication communication = (ICommunication)Activator.CreateInstance(type);

            bool res = (bool)communication.ConnectDeviceAndSettingWindow(this);
            if (res)
            {
                //todo:已连接，进入主控制页面

                throw new NotImplementedException();
            }
            else
            {
                //返回
                MessageBox.Show("未连接");
            }

        }
    }
}

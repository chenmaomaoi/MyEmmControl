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

namespace MyEmmControl.View
{
    /// <summary>
    /// SelectCommunicationMode_Window.xaml 的交互逻辑
    /// </summary>
    public partial class SelectCommunicationMode_Window : Window
    {
        private string prefix = null;

        public SelectCommunicationMode_Window()
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
            if (cbx_CommunicationType.Items.Count > 0)
            {
                cbx_CommunicationType.SelectedIndex = 0;
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
                App.MainWindow.controller = new EmmController.Controller(communication);

                this.Close();
            }
            else
            {
                //返回
                MessageBox.Show("未连接");
            }
        }
    }
}

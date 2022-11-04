using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MyEmmControl.Extensions;
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
        private Dictionary<string, Type> communicationNames = new Dictionary<string, Type>();

        public SelectCommunicationMode_Window()
        {
            InitializeComponent();

            var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes().Where(t => t.BaseType == typeof(CommunicationBase)));
            foreach (Type type in types)
            {
                string description = type.GetCustomAttribute<DescriptionAttribute>().Description;
                communicationNames.Add(description, type);
                cbx_CommunicationType.Items.Add(description);
            }

            if (cbx_CommunicationType.Items.Count > 0)
            {
                cbx_CommunicationType.SelectedIndex = 0;
            }
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            string typeName = cbx_CommunicationType.SelectedItem.ToString();
            communicationNames.TryGetValue(typeName, out Type communicationType);
            ICommunication communication = (ICommunication)Activator.CreateInstance(communicationType);
            bool res = communication.ConnectDeviceAndSettingWindow(this);
            if (res)
            {
                App.MainWindow.controller = new EmmController(communication);
                DialogResult = true;
                this.Close();
            }
            else
            {
                //返回
                MessageBox.Show("连接失败");
            }
        }
    }
}

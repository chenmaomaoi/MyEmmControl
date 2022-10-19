using System;
using System.Collections.Generic;
using System.Linq;
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
        private MyBLE ble;
        public MainWindow()
        {
            InitializeComponent();
            ble = new MyBLE();
            ble.DiscoverDevicesComplete += this.Ble_DiscoverDevicesComplete;
            ble.OnRecvdData += Ble_RecvdData;
        }

        private void Ble_RecvdData(object sender, byte[] args)
        {
            Dispatcher.Invoke(() => lab_Status.Content = Encoding.Default.GetString(args));
        }

        private void Ble_DiscoverDevicesComplete(object sender, List<DeviceInformation> e)
        {
            Dispatcher.Invoke(() =>
            {
                list_DiscoveredDevices.Items.Clear();
                foreach (DeviceInformation item in e)
                {
                    string name = string.IsNullOrWhiteSpace(item.Name) 
                                    ? item.Id.Split('-')[1] 
                                    : item.Name + "-" + item.Id.Split('-')[1];

                    list_DiscoveredDevices.Items.Add(name);

                    Console.WriteLine(name);
                }
                progress_Discover.Value = 0;
            });
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            ble.DiscoverDevices();

            Task.Run(() => 
            {
                double add = 100 / 30;
                for (int i = 0; i < 32; i++)
                {
                    Dispatcher.Invoke(() => progress_Discover.Value += add);
                    Thread.Sleep(1000);
                }
            });
        }

        private async void Item_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ListViewItem v = sender as ListViewItem;
            Console.WriteLine(v.DataContext);

            lab_Status.Content = "连接中..." + v.DataContext;
            string id = v.DataContext.ToString();
            if (id.Contains('-'))
            {
                id = id.Split('-')[1];
            }

            RetStatus ret = await ble.ConnectDevice(id);
            lab_Status.Content = ret.status + "-" + ret.message;

        }
    }
}

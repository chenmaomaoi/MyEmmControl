using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MyBLE;
using Windows.Devices.Enumeration;

namespace MyEmmControl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BLE ble;
        public MainWindow()
        {
            InitializeComponent();
            ble = new BLE();
            ble.DiscoverDevicesComplete += this.Ble_DiscoverDevicesComplete;

        }

        private void Ble_DiscoveredDevice(DeviceWatcher sender, DeviceInformation args)
        {
            list_DiscoveredDevices.Dispatcher.BeginInvoke(new Action(() =>
            {
                string[] s = new string[2];
                s[0] = args.Name;
                s[1] = args.Id.Split('-')[1];
                list_DiscoveredDevices.Items.Add(args.Id.Split('-')[1]);
            }));
        }

        private void Ble_DeviceInformationUpdate(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            list_DiscoveredDevices.Dispatcher.BeginInvoke(new Action(() =>
            {
                list_DiscoveredDevices.Items.Add(args.Id.Split('-')[1]);
            }));
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
            });
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            ble.DiscoverDevices();
        }

        private void Item_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var v = sender as ListViewItem;
            Console.WriteLine(v.DataContext);
        }

    }
}

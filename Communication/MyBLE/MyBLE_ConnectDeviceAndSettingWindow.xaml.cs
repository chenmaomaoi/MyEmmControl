using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.Devices.Enumeration;

namespace MyEmmControl.Communication
{
    /// <summary>
    /// MyBLE_ConnectDeviceAndSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MyBLE_ConnectDeviceAndSettingWindow : Window
    {
        private MyBLE ble;

        private bool flag_SearchDevicesComplete = false;

        public MyBLE_ConnectDeviceAndSettingWindow(MyBLE myBLE)
        {
            InitializeComponent();
            ble = myBLE;

            ble.DiscoverDevicesComplete += this.Ble_DiscoverDevicesComplete;
            ble.OnRecvdData += Ble_RecvdData;
        }

        private void Ble_RecvdData(object sender, byte[] args)
        {
            Dispatcher.Invoke(() => text.Text = Encoding.Default.GetString(args));
        }

        private void Ble_DiscoverDevicesComplete(object sender, List<DeviceInformation> e)
        {
            Dispatcher.Invoke(() =>
            {
                list_Devices.Items.Clear();
                foreach (DeviceInformation item in e)
                {
                    string name = string.IsNullOrWhiteSpace(item.Name)
                                    ? item.Id.Split('-')[1]
                                    : item.Name + "-" + item.Id.Split('-')[1];

                    list_Devices.Items.Add(name);

                    Console.WriteLine(name);
                }
                flag_SearchDevicesComplete = true;
                progress.Value = 0;
                text.Text = "";

                list_Devices.IsEnabled = true;
                btn_SearchDevices.IsEnabled = true;
            });
        }

        private void btn_SearchDevices_Click(object sender, RoutedEventArgs e)
        {
            list_Devices.IsEnabled = false;
            btn_SearchDevices.IsEnabled = false;
            btn_ConnectDevice.IsEnabled = false;
            text.Text = "搜索设备中...";
            ble.DiscoverDevices();
            flag_SearchDevicesComplete = false;
            Task.Run(() =>
            {
                double add = 100 / 30;
                for (int i = 0; i < 32; i++)
                {
                    if (flag_SearchDevicesComplete)
                    {
                        break;
                    }
                    else
                    {
                        Dispatcher.Invoke(() => progress.Value += add);
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        private async void btn_ConnectDevice_Click(object sender, RoutedEventArgs e)
        {
            string id = list_Devices.SelectedItem.ToString();

            bool ret = await connect(id);

            if (ret)
            {
                this.DialogResult = true;
            }
        }

        private async void Item_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ListViewItem v = sender as ListViewItem;
            Console.WriteLine(v.DataContext);

            string id = v.DataContext.ToString();

            bool ret = await connect(id);

            if (ret)
            {
                this.DialogResult = true;
            }
        }

        private async Task<bool> connect(string id)
        {
            text.Text = "连接中..." + id;
            if (id.Contains('-'))
            {
                id = id.Split('-')[1];
            }

            RetStatus ret = await ble.ConnectDevice(id);
            text.Text = ret.status + "-" + ret.message;

            await Task.Delay(1000);
            return ret.status;
        }

        private void list_Devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list_Devices.Items.Count <= 0 || list_Devices.SelectedIndex == -1)
            {
                btn_ConnectDevice.IsEnabled = false;
            }
            else
            {
                btn_ConnectDevice.IsEnabled = true;
            }
        }
    }
}

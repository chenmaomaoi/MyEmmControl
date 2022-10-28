using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyEmmControl.Communication
{
    /// <summary>
    /// MySerialPort_ConnectDeviceAndSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MySerialPort_ConnectDeviceAndSettingWindow : Window
    {
        private MySerialPort mySerialPort;

        public MySerialPort_ConnectDeviceAndSettingWindow(MySerialPort serialPort)
        {
            InitializeComponent();
            this.mySerialPort = serialPort;

            //获取串口列表
            cbx_SerialPortList.DataContext = SerialPort.GetPortNames();
            //设置可选波特率
            cbx_BaudRate.DataContext = new object[] { 9600, 19200, 115200 };
            //设置可选奇偶校验
            cbx_Parity.DataContext = new object[] { "None", "Odd", "Even", "Mark", "Space" };
            //设置可选数据位
            cbx_DataBits.DataContext = new object[] { 5, 6, 7, 8 };
            //设置可选停止位
            cbx_StopBits.DataContext = new object[] { 1, 1.5, 2 };
        }

        private async void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            mySerialPort.serialPort.PortName = cbx_SerialPortList.SelectedItem.ToString();
            mySerialPort.serialPort.BaudRate = int.Parse(cbx_BaudRate.SelectedItem.ToString());
            mySerialPort.serialPort.Parity =  GetSelectedParity();
            mySerialPort.serialPort.DataBits = int.Parse(cbx_DataBits.SelectedItem.ToString());
            mySerialPort.serialPort.StopBits = GetSelectedStopBits();
            try
            {
                mySerialPort.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("无法打开此串口，请检查是否被占用");
                return;
            }

            await Task.Delay(1000);
            this.DialogResult = true;
        }

        /// <summary>
        /// 获取窗体选中的奇偶校验
        /// </summary>
        /// <returns></returns>
        private Parity GetSelectedParity()
        {
            switch (cbx_Parity.SelectedItem.ToString())
            {
                case "Odd":
                    return Parity.Odd;
                case "Even":
                    return Parity.Even;
                case "Mark":
                    return Parity.Mark;
                case "Space":
                    return Parity.Space;
                case "None":
                default:
                    return Parity.None;
            }
        }

        /// <summary>
        /// 获取窗体选中的停止位
        /// </summary>
        /// <returns></returns>
        private StopBits GetSelectedStopBits()
        {
            switch (Convert.ToDouble(cbx_StopBits.SelectedItem))
            {
                case 1:
                    return StopBits.One;
                case 1.5:
                    return StopBits.OnePointFive;
                case 2:
                    return StopBits.Two;
                default:
                    return StopBits.One;
            }
        }

    }
}

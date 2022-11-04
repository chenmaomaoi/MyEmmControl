using MyEmmControl.Communication;
using MyEmmControl.Extensions;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MyEmmControl.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public EmmController controller;

        private Dictionary<string,ChecksumTypes> checksums = new Dictionary<string,ChecksumTypes>();

        public MainWindow()
        {
            InitializeComponent();

            foreach (ChecksumTypes checksumType in Enum.GetValues(typeof(ChecksumTypes)))
            {
                var typeDescription = checksumType.GetFieldAttribute<DescriptionAttribute>().Description;
                checksums.Add(typeDescription, checksumType);
                cbx_CheckType.Items.Add(typeDescription);
            }

            if (cbx_CheckType.Items.Count > 0)
            {
                cbx_CheckType.SelectedIndex = 0;
            }
        }

        private void AddLog(string text)
        {
            text_Log.Text += $@"[{DateTime.Now}] {text}{Environment.NewLine}";
        }

        private void btn_CalibrationEncoder_Click(object sender, RoutedEventArgs e)
        {
            AddLog(CommandHeads.CalibrationEncoder.ToString());
            var result = controller.SendCommand(CommandHeads.CalibrationEncoder);
            AddLog(result);
        }

        private void btn_ConnectDevice_Click(object sender, RoutedEventArgs e)
        {
            Window selectCommunicationMode = new SelectCommunicationMode_Window();
            selectCommunicationMode.Owner = this;
            selectCommunicationMode.ShowDialog();
            grid_Content.IsEnabled = controller != null;
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            //发送命令
        }

        private void cbx_CheckType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( checksums.TryGetValue(cbx_CheckType.SelectedIndex.ToString(), out var type))
            {
                controller.ChecksumType = type;
            }
        }

        private void radio_SpeedPercentMode_Checked(object sender, RoutedEventArgs e)
        {
            //速度 百分比模式和绝对值模式
        }

        private void radio_AcceleratePercentMode_Checked(object sender, RoutedEventArgs e)
        {
            //加速度
        }

        private void radio_AngleMode_Checked(object sender, RoutedEventArgs e)
        {
            //转角/脉冲
        }
    }
}

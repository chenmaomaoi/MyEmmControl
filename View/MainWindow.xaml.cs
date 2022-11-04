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

        public MainWindow()
        {
            InitializeComponent();
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
        }
    }
}

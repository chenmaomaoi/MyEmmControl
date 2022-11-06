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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public EmmController controller;

        private Dictionary<string, ChecksumTypes> checksums = new Dictionary<string, ChecksumTypes>();

        private double _speedValue;
        public double SpeedValue
        {
            get => _speedValue;
            set
            {
                _speedValue = value;
                OnPropertyChanged(nameof(SpeedValue));
            }
        }

        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

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

            mySlider_Speed.Maximum = 300;
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
            var v = mySlider_Speed.IsUsePercent;

        }

        private void cbx_CheckType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (checksums.TryGetValue(cbx_CheckType.SelectedIndex.ToString(), out var type))
            {
                controller.ChecksumType = type;
            }
        }
    }
}

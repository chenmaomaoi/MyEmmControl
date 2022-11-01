using System;
using System.Collections.Generic;
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
using MyEmmControl.EmmController;

namespace MyEmmControl.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public Controller controller;

        public MainWindow()
        {
            InitializeComponent();

            var ty = typeof(DirectionOfRotation);
            ty.GetEnumValues();

            foreach (var item in ty.GetEnumValues())
            {
                combo_Direction.Items.Add(item.ToString());
            }

            if (combo_Direction.Items.Count > 0)
            {
                combo_Direction.SelectedIndex = 0;
            }
        }

        private void btn_CalibrationEncoder_Click(object sender, RoutedEventArgs e)
        {
            controller.SendCommand(CommandHead.CalibrationEncoder);
        }

        private void btn_ConnectDevice_Click(object sender, RoutedEventArgs e)
        {
            //todo:判断是否连接成功。
            new SelectCommunicationMode_Window().ShowDialog();
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            var v = new CommandBody();
            v.Direction = (DirectionOfRotation)Enum.Parse(typeof(DirectionOfRotation), combo_Direction.SelectedItem.ToString());
            v.Speed = Convert.ToUInt16(text_Speed.Text);
            v.Acceleration = Convert.ToByte(text_Acceleration.Text);

            controller.SendCommand(CommandHead.SetRotation, v);
        }
    }
}

using MyEmmControl.Attributes;
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

        Dictionary<string, DirectionOfRotation> directionOfRoatationNames = new Dictionary<string, DirectionOfRotation>();

        public MainWindow()
        {
            InitializeComponent();

            foreach (var item in typeof(DirectionOfRotation).GetEnumValues())
            {
                string description = item.GetFieldAttribute<DescriptionAttribute>().Description;
                directionOfRoatationNames.Add(description, (DirectionOfRotation)item);
                combo_Direction.Items.Add(description);
            }

            if (combo_Direction.Items.Count > 0)
            {
                combo_Direction.SelectedIndex = 0;
            }
        }

        private void btn_CalibrationEncoder_Click(object sender, RoutedEventArgs e)
        {
            controller.SendCommand(CommandHeads.CalibrationEncoder);
        }

        private void btn_ConnectDevice_Click(object sender, RoutedEventArgs e)
        {
            //todo:判断是否连接成功。
            new SelectCommunicationMode_Window().ShowDialog();
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            directionOfRoatationNames.TryGetValue(combo_Direction.SelectedItem.ToString(), out var directionOfRotation);

            CommandBody cmdBody = new CommandBody
            {
                Direction = directionOfRotation,
                Speed = Convert.ToUInt16(text_Speed.Text),
                Acceleration = Convert.ToByte(text_Acceleration.Text)
            };
            controller.SendCommand(CommandHeads.SetRotation, cmdBody);
        }
    }
}

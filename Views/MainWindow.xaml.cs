using Microsoft.Extensions.DependencyInjection;
using MyEmmControl.Communication;
using MyEmmControl.Extensions;
using MyEmmControl.ViewModes;
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

namespace MyEmmControl.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = App.Host.Services.GetRequiredService<SelectCommunicationMode_Window>();
            window.Owner = this;
            window.DataContext = this.DataContext;
            window.ShowDialog();
        }
    }
}

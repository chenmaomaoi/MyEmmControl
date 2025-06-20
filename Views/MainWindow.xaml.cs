using System;
using System.Windows;

namespace MyEmmControl.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //private readonly SelectCommunicationMode_Window Mode_Window;

        public MainWindow()
        {
            InitializeComponent();
            //Mode_Window = mode_Window;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectCommunicationMode_Window Mode_Window = new SelectCommunicationMode_Window
            {
                Owner = this,
                DataContext = this.DataContext
            };
            Mode_Window.ShowDialog();
        }
    }
}

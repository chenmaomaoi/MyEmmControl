using System.Windows;

namespace MyEmmControl.Views
{
    /// <summary>
    /// SelectCommunicationMode_Window.xaml 的交互逻辑
    /// </summary>
    public partial class SelectCommunicationMode_Window : Window
    {
        public SelectCommunicationMode_Window()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

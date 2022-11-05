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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyEmmControl.View.MyControl
{
    /// <summary>
    /// MySlider.xaml 的交互逻辑
    /// </summary>
    public partial class MySlider : UserControl, INotifyPropertyChanged
    {
        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double Value
        {
            get
            {
                OnPropertyChanged(nameof(Value));
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
                OnPropertyChanged(nameof(Value));
            }
        }
        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                                        typeof(double),
                                        typeof(MySlider),
                                        new PropertyMetadata(0.0));

        public event PropertyChangedEventHandler PropertyChanged;

        public bool UsePercent
        {
            get => (bool)GetValue(UsePercentProperty);
            set { SetValue(UsePercentProperty, value); OnPropertyChanged(nameof(UsePercent)); }
        }
        // Using a DependencyProperty as the backing store for UsePercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsePercentProperty =
            DependencyProperty.Register("UsePercent",
                                        typeof(bool),
                                        typeof(MySlider),
                                        new PropertyMetadata(true));


        public MySlider()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}

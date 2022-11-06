using System.Windows;
using System.Windows.Controls;

namespace MyEmmControl.View.MyControl
{
    /// <summary>
    /// MySlider.xaml 的交互逻辑
    /// </summary>
    public partial class MySlider : UserControl
    {
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value),
                                        typeof(double),
                                        typeof(MySlider),
                                        new PropertyMetadata(default(double)));

        public bool IsUsePercent
        {
            get => (bool)GetValue(IsUsePercentProperty);
            set
            {
                SetValue(IsUsePercentProperty, value);
            }
        }
        // Using a DependencyProperty as the backing store for UsePercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUsePercentProperty =
            DependencyProperty.Register(nameof(IsUsePercent),
                                        typeof(bool),
                                        typeof(MySlider),
                                        new PropertyMetadata(true));


        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum),
                                        typeof(int),
                                        typeof(MySlider),
                                        new PropertyMetadata(default(int)));


        public MySlider()
        {
            InitializeComponent();
        }

        private bool _firstTime = true;
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_firstTime)
            {
                _firstTime = false;
            }
            else
            {
                if (IsUsePercent)
                {
                    slider.Maximum = 100;
                    slider.SmallChange = 0.1;
                    slider.TickFrequency = 0.1;
                }
                else
                {
                    slider.Maximum = Maximum;
                    slider.SmallChange = 1;
                    slider.TickFrequency = 1;
                }
            }
        }
    }
}

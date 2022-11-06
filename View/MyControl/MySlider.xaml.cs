using System.Windows;
using System.Windows.Controls;

namespace MyEmmControl.View.MyControl
{
    /// <summary>
    /// MySlider.xaml 的交互逻辑
    /// </summary>
    public partial class MySlider : UserControl
    {
        //根据模式返回具体的值
        public double Value => IsRelativeMode ? (double)((double)AbsoluteMaximum / RelativeMaximum) * slider.Value
                                              : slider.Value;

        private double value
        {
            get => (double)GetValue(valueProperty);
            set => SetValue(valueProperty, value);
        }
        public static readonly DependencyProperty valueProperty =
            DependencyProperty.Register(nameof(value),
                                        typeof(double),
                                        typeof(MySlider),
                                        new PropertyMetadata(default(double)));



        public bool IsRelativeMode
        {
            get => (bool)GetValue(IsRelativeProperty);
            set => SetValue(IsRelativeProperty, value);
        }
        public static readonly DependencyProperty IsRelativeProperty =
            DependencyProperty.Register(nameof(IsRelativeMode),
                                        typeof(bool),
                                        typeof(MySlider),
                                        new PropertyMetadata(default(bool)));

        public string RelativeText
        {
            get => (string)GetValue(RelativeTextProperty);
            set => SetValue(RelativeTextProperty, value);
        }
        public static readonly DependencyProperty RelativeTextProperty =
            DependencyProperty.Register(nameof(RelativeText),
                                        typeof(string),
                                        typeof(MySlider),
                                        new PropertyMetadata(default(string)));

        public string AbsoluteText
        {
            get => (string)GetValue(AbsoluteTextProperty);
            set => SetValue(AbsoluteTextProperty, value);
        }
        public static readonly DependencyProperty AbsoluteTextProperty =
            DependencyProperty.Register(nameof(AbsoluteText),
                                        typeof(string),
                                        typeof(MySlider),
                                        new PropertyMetadata(default(string)));

        public int RelativeMaximum
        {
            get => (int)GetValue(RelativeProperty);
            set => SetValue(RelativeProperty, value);
        }
        public static readonly DependencyProperty RelativeProperty =
            DependencyProperty.Register(nameof(RelativeMaximum),
                                        typeof(int),
                                        typeof(MySlider),
                                        new PropertyMetadata(default(int)));

        public int AbsoluteMaximum
        {
            get => (int)GetValue(AbsoluteMaximumProperty);
            set => SetValue(AbsoluteMaximumProperty, value);
        }
        public static readonly DependencyProperty AbsoluteMaximumProperty =
            DependencyProperty.Register(nameof(AbsoluteMaximum),
                                        typeof(int),
                                        typeof(MySlider),
                                        new PropertyMetadata(default(int)));

        private int maximum
        {
            get => IsRelativeMode ? RelativeMaximum : AbsoluteMaximum;
            set
            {
                if (IsRelativeMode)
                {
                    RelativeMaximum = value;
                }
                else
                {
                    AbsoluteMaximum = value;
                }
            }
        }

        public MySlider()
        {
            InitializeComponent();
            RelativeText = "相对值";
            RelativeMaximum = 100;
            AbsoluteText = "绝对值";
            AbsoluteMaximum = 300;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRelativeMode)
            {
                slider.SmallChange = 0.1;
                slider.TickFrequency = 0.1;
                slider.Value = slider.Value / ((double)AbsoluteMaximum / RelativeMaximum);
                slider.Maximum = RelativeMaximum;
            }
            else
            {
                slider.Maximum = AbsoluteMaximum;
                slider.SmallChange = 1;
                slider.TickFrequency = 1;
                slider.Value = (int)((double)AbsoluteMaximum / RelativeMaximum * slider.Value);
            }
        }
    }
}

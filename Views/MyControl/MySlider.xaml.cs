using System;
using System.Windows;
using System.Windows.Controls;

namespace MyEmmControl.Views.MyControl
{
    /// <summary>
    /// MySlider.xaml 的交互逻辑
    /// </summary>
    public partial class MySlider : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Get或Set都是绝对值</remarks>
        public int Value
        {
            get
            {
                return IsRelativeMode ? (int)((double)AbsoluteMaximum / RelativeMaximum * (double)GetValue(ValueProperty))
                                      : (int)(double)GetValue(ValueProperty);
            }
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value),
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
                                        new PropertyMetadata("相对值"));

        public string AbsoluteText
        {
            get => (string)GetValue(AbsoluteTextProperty);
            set => SetValue(AbsoluteTextProperty, value);
        }
        public static readonly DependencyProperty AbsoluteTextProperty =
            DependencyProperty.Register(nameof(AbsoluteText),
                                        typeof(string),
                                        typeof(MySlider),
                                        new PropertyMetadata("绝对值"));

        public int RelativeMaximum
        {
            get => (int)GetValue(RelativeProperty);
            set => SetValue(RelativeProperty, value);
        }
        public static readonly DependencyProperty RelativeProperty =
            DependencyProperty.Register(nameof(RelativeMaximum),
                                        typeof(int),
                                        typeof(MySlider),
                                        new PropertyMetadata(100));

        public int AbsoluteMaximum
        {
            get => (int)GetValue(AbsoluteMaximumProperty);
            set => SetValue(AbsoluteMaximumProperty, value);
        }
        public static readonly DependencyProperty AbsoluteMaximumProperty =
            DependencyProperty.Register(nameof(AbsoluteMaximum),
                                        typeof(int),
                                        typeof(MySlider),
                                        new PropertyMetadata(200));

        private double step => (double)AbsoluteMaximum / RelativeMaximum;

        public MySlider()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRelativeMode)
            {
                slider.SmallChange = 0.1;
                slider.TickFrequency = 0.1;

                //绝对值转引用
                slider.Value = Math.Round(slider.Value / step, 1);
                slider.Maximum = RelativeMaximum;
            }
            else
            {
                slider.Maximum = AbsoluteMaximum;
                slider.SmallChange = 1;
                slider.TickFrequency = 1;

                //引用转绝对值
                slider.Value = (int)(slider.Value * step);
            }
        }
    }
}

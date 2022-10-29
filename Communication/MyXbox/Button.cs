using System;
using System.Reflection;
using System.Timers;

namespace MyEmmControl.Communication
{
    public static class AttributeEx
    {
        public static TAtteibute GetAttribute<TAtteibute>(this object obj) where TAtteibute : Attribute
        {
            Type type = obj.GetType();
            //获取字段信息
            FieldInfo field = type.GetField(obj.ToString());
            //检查字段是否含有指定特性
            if (field.IsDefined(typeof(TAtteibute), true))
            {
                //获取字段上的自定义特性
                TAtteibute remarkAttribute = (TAtteibute)field.GetCustomAttribute(typeof(TAtteibute));
                return remarkAttribute;
            }
            else
            {
                return null;
            }
        }
    }

    public class ButtonAttribute : Attribute
    {
        public string ButtonName { get; set; }
        public ButtonAttribute(string buttonName)
        {
            ButtonName = buttonName;
        }
    }

    public partial class MyXbox
    {
        /// <summary>
        /// Xbox按键
        /// </summary>
        public enum ButtonName
        {
            [Button("LeftShoulder")] LB,
            [Button("LeftThumb")] LS,
            [Button("Back")] BACK,
            [Button("DPadUp")] UP,
            [Button("DPadRight")] RIGHT,
            [Button("DPadDown")] DOWN,
            [Button("DPadLeft")] LEFT,
            [Button("RightShoulder")] RB,
            [Button("RightThumb")] RS,
            [Button("Start")] START,
            [Button("X")] X,            
            [Button("Y")] Y,
            [Button("A")] A,
            [Button("B")] B
        }

        public class Button
        {
            public ButtonName Name { get; private set; }

            public Button(ButtonName name)
            {
                Name = name;
                timer = new Timer() { Interval = 300, AutoReset = true };
                timer.Elapsed += Timer_Elapsed;
            }

            private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                timer.Interval = 100;
                ButtonClick?.Invoke(this, null);
            }

            private Timer timer;

            private bool isPressed;
            public bool IsPressed
            {
                get => isPressed;
                set
                {
                    if (isPressed == value) return;

                    if (value)
                    {
                        timer.Start();
                        ButtonDown?.Invoke(this, null);
                        ButtonClick?.Invoke(this, null);
                    }
                    else
                    {
                        timer.Stop();
                        timer.Interval = 300;
                        ButtonUp?.Invoke(this, null);
                    }

                    isPressed = value;
                }
            }

            public event EventHandler ButtonDown;
            public event EventHandler ButtonClick;
            public event EventHandler ButtonUp;
        }
    }
}

using System;
using System.Timers;

namespace MyEmmControl.Communication
{
    public partial class MyXbox
    {
        /// <summary>
        /// Xbox按键
        /// </summary>
        public enum ButtonName
        {
            LB, LS,
            BACK,
            UP, RIGHT, DOWN, LEFT,

            RB, RS,
            START,
            X, Y, A, B
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

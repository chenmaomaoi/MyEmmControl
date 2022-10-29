using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace MyEmmControl.Communication
{
    public partial class MyXbox
    {
        public event EventHandler<byte> LTValueChanged;
        private byte lt = 0;
        public byte LT
        {
            get => lt;
            private set
            {
                if (useFilter)
                {
                    byte tmp = (byte)kalman[0].filter(value);
                    if (lt == tmp) return;
                    lt = tmp;
                }
                else
                {
                    if (lt == value) return;
                    lt = value;
                }
                LTValueChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<byte> RTValueChanged;
        private byte rt = 0;
        public byte RT
        {
            get => rt;
            private set
            {
                if (useFilter)
                {
                    byte tmp = (byte)kalman[1].filter(value);
                    if (rt == tmp) return;
                    rt = tmp;
                }
                else
                {
                    if (rt == value) return;
                    rt = value;
                }
                RTValueChanged?.Invoke(this, value);
            }
        }

        public StickValue LS { get; private set; }
        public event EventHandler<StickValue> LSValueChanged;

        public StickValue RS { get; private set; }
        public event EventHandler<StickValue> RSValueChanged;

        public List<Button> Buttons = new List<Button>();
        public event EventHandler<List<Button>> ButtonPress;

        /// <summary>
        /// 卡尔曼滤波
        /// </summary>
        private FilterKalman[] kalman = new FilterKalman[6];
        private bool useFilter;

        public readonly Controller Controller;

        /// <summary>
        /// 震动
        /// </summary>
        /// <param name="vibration"></param>
        /// <returns></returns>
        public bool SetVibration(Vibration vibration) => Controller.SetVibration(vibration).Success;

        private void initkalman()
        {
            //LT  RT
            for (int i = 0; i < 2; i++) kalman[i] = new FilterKalman(1, 0xFFFFFFFF);

            //LS  RS
            for (int i = 2; i < kalman.Length; i++) kalman[i] = new FilterKalman(1, 0xFFFFFF);
        }

        public MyXbox(bool useFilter = false)
        {
            State state;
            GamepadButtonFlags buttonFlags = new GamepadButtonFlags();

            this.useFilter = useFilter;
            initkalman();
            LS = new StickValue(kalman[2], kalman[3], useFilter);
            RS = new StickValue(kalman[4], kalman[5], useFilter);

            LS.StickValueChanged += (sender, e) => LSValueChanged?.Invoke(sender, e);
            RS.StickValueChanged += (sender, e) => RSValueChanged?.Invoke(sender, e);

            foreach (var item in typeof(ButtonName).GetEnumValues())
            {
                Buttons.Add(new Button((ButtonName)item));
            }

            Controller = new Controller(UserIndex.One);
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(5);    //防止过高资源占用
                    if (!Controller.IsConnected)
                    {
                        Thread.Sleep(3000);
                        continue;
                    }

                    state = Controller.GetState();

                    LT = state.Gamepad.LeftTrigger;
                    RT = state.Gamepad.RightTrigger;

                    LS.X = state.Gamepad.LeftThumbX;
                    LS.Y = state.Gamepad.LeftThumbY;
                    RS.X = state.Gamepad.RightThumbX;
                    RS.Y = state.Gamepad.RightThumbY;

                    //绑定button isPressed
                    if (buttonFlags == state.Gamepad.Buttons)
                    {
                        continue;
                    }
                    buttonFlags = state.Gamepad.Buttons;

                    Type _attType = typeof(ButtonAttribute);
                    Type _buttonFlagType = typeof(GamepadButtonFlags);

                    foreach (Button button in Buttons)
                    {
                        //遍历每个按钮，获取状态变化
                        ButtonAttribute att = button.Name.GetAttribute<ButtonAttribute>(_attType);
                        string name = att?.ButtonName;

                        GamepadButtonFlags flag = (GamepadButtonFlags)Enum.Parse(_buttonFlagType, name);
                        button.IsPressed = ((buttonFlags & flag) == flag);
                    }

                    if (buttonFlags != GamepadButtonFlags.None)
                    {
                        ButtonPress?.Invoke(this, Buttons.FindAll((Button b) => b.IsPressed));
                    }
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyEmmControl.Communication;

namespace MyEmmControl.ViewModes
{
    public class MainWindowViewMode : ObservableObject
    {
        public EmmController Controller { get; set; }

        /// <summary>
        /// 设备是否连接
        /// </summary>
        public bool IsConnected { get => _isConnected; set => SetProperty(ref _isConnected, value); }
        private bool _isConnected = true;

        /// <summary>
        /// 是否为速度模式
        /// </summary>
        public bool IsSpeedMode { get => _isSpeedMode; set => SetProperty(ref _isSpeedMode, value); }
        private bool _isSpeedMode = true;

        /// <summary>
        /// 是否为顺时针转动
        /// </summary>
        public bool IsClockwiseRotation { get => _isClockwiseRotation; set => SetProperty(ref _isClockwiseRotation, value); }
        private bool _isClockwiseRotation = true;

        /// <summary>
        /// 速度
        /// </summary>
        public ushort Speed { get => _speed; set => SetProperty(ref _speed, value); }
        private ushort _speed = default;

        /// <summary>
        /// 加速度
        /// </summary>
        public byte Acceleration { get => _acceleration; set => SetProperty(ref _acceleration, value); }
        private byte _acceleration = default;

        /// <summary>
        /// 脉冲数
        /// </summary>
        public uint Puls { get => _puls; set => SetProperty(ref _puls, value); }
        private uint _puls = default;

        public MainWindowViewMode()
        {
            Controller = new EmmController(new MyBLE());
            ConnectDeviceCommand = new AsyncRelayCommand(ConnectDevice);
        }

        public ICommand ConnectDeviceCommand { get; }

        public async Task ConnectDevice()
        {
            await Task.Delay(10);
            MessageBox.Show("点击了按钮");
            return;
        }
    }
}

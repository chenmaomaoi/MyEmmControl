using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MyEmmControl.Communication;
using MyEmmControl.Views;

namespace MyEmmControl.ViewModes
{
    public class MainViewMode : ObservableObject
    {
        public EmmController Controller { get; set; }

        /// <summary>
        /// 设备是否连接
        /// </summary>
        public bool IsConnected { get => _isConnected; set => SetProperty(ref _isConnected, value); }
        private bool _isConnected = false;

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

        public MainViewMode()
        {
            MainWindow = App.Host.Services.GetRequiredService<MainWindow>();

            //初始化通信模式选项
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.BaseType == typeof(CommunicationBase)))
                .Reverse();
            foreach (Type type in types)
            {
                CommunicationTypes.Add(type.GetCustomAttribute<DescriptionAttribute>().Description, type);
            }
            //初始化Command
            ConnectCommand = new RelayCommand<Type>(Connect);
        }

        private MainWindow MainWindow { get; }

        public ICommand SelectDeviceCommand { get; }

        public Dictionary<string, Type> CommunicationTypes { get; private set; } = new Dictionary<string, Type>();

        public ICommand ConnectCommand { get; }
        private void Connect(Type args)
        {
            IsConnected = false;
            ICommunication communication = (ICommunication)Activator.CreateInstance(args);
            IsConnected = communication.ConnectDeviceAndSettingWindow(this.MainWindow);
            Controller = IsConnected ? new EmmController(communication) : null;
        }
    }
}

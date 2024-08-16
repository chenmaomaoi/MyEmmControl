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
using Microsoft.Extensions.Logging;
using MyEmmControl.Communication;
using MyEmmControl.Extensions;
using MyEmmControl.Views;

namespace MyEmmControl.ViewModes
{
    public class MainViewMode : ObservableObject
    {
        public EmmController Controller { get => _controller; private set => SetProperty(ref _controller, value); }
        private EmmController _controller;

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

        private readonly ILogger<MainViewMode> _logger;

        /// <summary>
        /// 速度
        /// </summary>
        public ushort Speed { get => (ushort)MainWindow.slider_Speed.Value; }

        /// <summary>
        /// 加速度
        /// </summary>
        public byte Acceleration { get => (byte)MainWindow.slider_Acceleration.Value; }

        /// <summary>
        /// 脉冲数
        /// </summary>
        public uint Puls { get => (uint)MainWindow.slider_Puls.Value; }

        public MainViewMode(MainWindow mainWindow, ILogger<MainViewMode> logger)
        {
            MainWindow = mainWindow;
            MainWindow.DataContext = this;
            _logger = logger;
            _logger.LogInformation("1234");

            //初始化通信模式选项
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.BaseType == typeof(CommunicationBase)))
                .Reverse();
            foreach (Type type in types)
            {
                CommunicationTypes.Add(type.GetCustomAttribute<DescriptionAttribute>().Description, type);
            }
            //初始化校验位选项
            foreach (ChecksumTypes checksumType in Enum.GetValues(typeof(ChecksumTypes)))
            {
                DataChecksumTypes.Add(checksumType.GetFieldAttribute<DescriptionAttribute>().Description, checksumType);
            }

            //初始化Command
            ConnectCommand = new RelayCommand<Type>(Connect);
            SendCommand = new RelayCommand<string>(Send);
            SetSubdivisionCommand = new RelayCommand<string>(SetSubdivision);
            SetUARTAddrCommand = new RelayCommand<string>(SetUARTAddr);
            SetMotionCommand = new RelayCommand(SetMotion);

            this.MainWindow.Show();
        }

        private MainWindow MainWindow { get; }

        public ICommand SelectDeviceCommand { get; }

        public Dictionary<string, Type> CommunicationTypes { get; private set; } = new Dictionary<string, Type>();
        public Dictionary<string, ChecksumTypes> DataChecksumTypes { get; private set; } = new Dictionary<string, ChecksumTypes>();

        public ICommand ConnectCommand { get; }
        private void Connect(Type args)
        {
            IsConnected = false;
            ICommunication communication = (ICommunication)Activator.CreateInstance(args);
            IsConnected = communication.ConnectDeviceAndSettingWindow(this.MainWindow);
            Controller = IsConnected ? new EmmController(communication) : null;
        }

        public ICommand SendCommand { get; }
        private void Send(string cmdHeads)
        {
            if (IsConnected)
            {
                Controller.SendCommand((EmmCmdHeads)Enum.Parse(typeof(EmmCmdHeads), cmdHeads));
            }
        }

        public ICommand SetSubdivisionCommand { get; }
        private void SetSubdivision(string subdivision)
        {
            if (IsConnected)
            {
                Controller.SendCommand(EmmCmdHeads.UpdateSubdivision, new EmmCmdBody() { Data = Convert.ToByte(subdivision) });
            }
        }

        public ICommand SetUARTAddrCommand { get; }
        private void SetUARTAddr(string uartAddr)
        {
            if (IsConnected)
            {
                Controller.SendCommand(EmmCmdHeads.UpdateSubdivision, new EmmCmdBody() { Data = Convert.ToByte(uartAddr) });
            }
        }

        public ICommand SetMotionCommand { get; }
        private void SetMotion()
        {
            if (!IsConnected) return;
            if (IsSpeedMode)
            {
                Controller.SendCommand(EmmCmdHeads.SetRotation,
                                       new EmmCmdBody()
                                       {
                                           Direction = this.IsClockwiseRotation ? DirectionOfRotation.CW : DirectionOfRotation.CCW,
                                           Speed = this.Speed,
                                           Acceleration = this.Acceleration
                                       });
            }
            else
            {
                Controller.SendCommand(EmmCmdHeads.SetPosition,
                                       new EmmCmdBody()
                                       {
                                           Direction = this.IsClockwiseRotation ? DirectionOfRotation.CW : DirectionOfRotation.CCW,
                                           Speed = this.Speed,
                                           Acceleration = this.Acceleration,
                                           PulsTimes = this.Puls
                                       });
            }
        }
    }
}

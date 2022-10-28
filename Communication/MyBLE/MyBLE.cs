using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Security.Cryptography;

namespace MyEmmControl.Communication
{
    public struct RetStatus
    {
        public bool status;
        public string message;

        public RetStatus(bool status, string message)
        {
            this.status = status;
            this.message = message;
        }
    }

    public class MyBLE : ICommunication
    {
        #region 属性
        /// <summary>
        /// 当前连接的设备MAC
        /// </summary>
        public string CurrentDeviceMAC { get; set; }

        /// <summary>
        /// 当前设备
        /// </summary>
        public BluetoothLEDevice CurrentDevice { get; set; }

        /// <summary>
        /// 主服务
        /// </summary>
        public GattDeviceService CurrentService { get; set; }

        /// <summary>
        /// 写特征对象
        /// </summary>
        public GattCharacteristic CurrentWriteCharacteristic { get; set; }

        /// <summary>
        /// 通知特征对象
        /// </summary>
        public GattCharacteristic CurrentNotifyCharacteristic { get; set; }

        /// <summary>
        /// 周围设备
        /// </summary>
        public List<DeviceInformation> DevicesInformation { get; set; } = new List<DeviceInformation>();

        public string ServiceGuid { get; private set; }
        public string WriteCharacteristicGuid { get; private set; }
        public string NotifyCharacteristicGuid { get; private set; }
        #endregion

        #region 事件
        /// <summary>
        /// 搜索设备完毕
        /// </summary>
        /// <remarks>线程已切换</remarks>
        public event EventHandler<List<DeviceInformation>> DiscoverDevicesComplete;

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <remarks>线程已切换</remarks>
        public event EventHandler<byte[]> OnRecvdData;
        #endregion

        #region 常量
        private const int CHARACTERISTIC_INDEX = 0;
        private const string AQS_ALL_BLUETOOTHLE_DEVICES = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";
        /// <summary>
        /// //特性通知类型通知启用
        /// </summary>
        private const GattClientCharacteristicConfigurationDescriptorValue
            CHARACTERISTIC_NOTIFICATION_TYPE = GattClientCharacteristicConfigurationDescriptorValue.Notify;

        private readonly string[] REQUESTED_PROPERTIES =
            {
                "System.Devices.Aep.DeviceAddress",
                "System.Devices.Aep.IsConnected",
                "System.Devices.Aep.Bluetooth.Le.IsConnectable"
            };
        #endregion

        private DeviceWatcher watcher { get; set; }

        #region 公开方法
        public MyBLE(string serviceGuid = "0000ffe0-0000-1000-8000-00805f9b34fb",
                     string writeCharacteristicGuid = "0000ffe1-0000-1000-8000-00805f9b34fb",
                     string notifyCharacteristicGuid = "0000ffe1-0000-1000-8000-00805f9b34fb")
        {
            this.ServiceGuid = serviceGuid;
            this.WriteCharacteristicGuid = writeCharacteristicGuid;
            this.NotifyCharacteristicGuid = notifyCharacteristicGuid;

            watcher = DeviceInformation.CreateWatcher(AQS_ALL_BLUETOOTHLE_DEVICES,
                                                      REQUESTED_PROPERTIES,
                                                      DeviceInformationKind.AssociationEndpoint);
            watcher.Added += watcher_Added;
            watcher.EnumerationCompleted += watcher_EnumerationCompleted;
        }

        public MyBLE()
        {
            this.ServiceGuid = "0000ffe0-0000-1000-8000-00805f9b34fb";
            this.WriteCharacteristicGuid = "0000ffe1-0000-1000-8000-00805f9b34fb";
            this.NotifyCharacteristicGuid = "0000ffe1-0000-1000-8000-00805f9b34fb";

            watcher = DeviceInformation.CreateWatcher(AQS_ALL_BLUETOOTHLE_DEVICES,
                                                      REQUESTED_PROPERTIES,
                                                      DeviceInformationKind.AssociationEndpoint);
            watcher.Added += watcher_Added;
            watcher.EnumerationCompleted += watcher_EnumerationCompleted;
        }

        /// <summary>
        /// 搜索周围BLE设备
        /// </summary>
        public void DiscoverDevices()
        {
            DevicesInformation.Clear();
            watcher.Start();
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="information"></param>
        /// <returns></returns>
        public Task<RetStatus> ConnectDevice(DeviceInformation information) => connectDevice(information.Id);

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="MAC"></param>
        /// <returns></returns>
        public Task<RetStatus> ConnectDevice(string MAC) => connectDevice(MAC);

        /// <summary>
        /// 主动断开连接
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            CurrentDeviceMAC = null;
            CurrentService?.Dispose();
            CurrentDevice?.Dispose();
            CurrentDevice = null;
            CurrentService = null;
            CurrentWriteCharacteristic = null;
            CurrentNotifyCharacteristic = null;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            if (CurrentWriteCharacteristic != null)
            {
                Task.Run(() => CurrentWriteCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data),
                                                                          GattWriteOption.WriteWithResponse));
            }
        }

        public bool? ConnectDeviceAndSettingWindow()
        {
            //设置与连接
            return new MyBLE_ConnectDeviceAndSettingWindow(this).ShowDialog();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 发现新设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void watcher_Added(DeviceWatcher sender, DeviceInformation args) => DevicesInformation.Add(args);

        /// <summary>
        /// 搜索设备完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void watcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            sender.Stop();
            await UniTask.SwitchToThreadPool();
            DiscoverDevicesComplete?.Invoke(sender, DevicesInformation);
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="MAC"></param>
        /// <returns></returns>
        private async Task<RetStatus> connectDevice(string MAC)
        {
            RetStatus retStatus = new RetStatus();

            CurrentDevice = null;
            string id = MAC;
            if (MAC.StartsWith("BluetoothLE#BluetoothLE"))
            {
                CurrentDeviceMAC = MAC.Split('-')[1];
            }
            else
            {
                CurrentDeviceMAC = MAC;

                //获取主机蓝牙MAC地址
                BluetoothAdapter mBluetoothAdapter = await BluetoothAdapter.GetDefaultAsync();
                byte[] _tmp = BitConverter.GetBytes(mBluetoothAdapter.BluetoothAddress);//ulong转换为byte数组
                Array.Reverse(_tmp);
                string macAddress = BitConverter.ToString(_tmp, 2, 6).Replace('-', ':').ToLower();

                //拼接ID
                id = "BluetoothLE#BluetoothLE" + macAddress + "-" + MAC;
            }

            try
            {
                CurrentDevice = await BluetoothLEDevice.FromIdAsync(id);
                //CurrentDevice.ConnectionStatusChanged += CurrentDevice_ConnectionStatusChanged;
                retStatus = await selectDeviceService();
            }
            catch
            {
                //找不到该设备
                retStatus.status = false;
                retStatus.message = "找不到设备";
            }

            return retStatus;
        }

        /// <summary>
        /// 按GUID 查找主服务
        /// </summary>
        /// <param name="characteristic">GUID 字符串</param>
        /// <returns></returns>
        private async Task<RetStatus> selectDeviceService()
        {
            RetStatus retStatus = new RetStatus();

            Guid guid = new Guid(ServiceGuid);
            try
            {
                CurrentService = (await CurrentDevice.GetGattServicesForUuidAsync(guid)).Services[CHARACTERISTIC_INDEX];
                if (CurrentService != null)
                {
                    retStatus = await getCurrentCharacteristic();
                }
            }
            catch (Exception)
            {
                retStatus.status = false;
                retStatus.message = "没有发现服务";
            }

            return retStatus;
        }

        private async Task<RetStatus> getCurrentCharacteristic()
        {
            RetStatus retStatus = new RetStatus();
            Guid writeGuid = new Guid(WriteCharacteristicGuid);
            Guid notifyGuid = new Guid(NotifyCharacteristicGuid);

            GattCharacteristicsResult writeCharacteristic = await CurrentService.GetCharacteristicsForUuidAsync(writeGuid);
            GattCharacteristicsResult notifyCharacteristic = await CurrentService.GetCharacteristicsForUuidAsync(notifyGuid);

            if (writeCharacteristic.Characteristics.Count > 0 && notifyCharacteristic.Characteristics.Count > 0)
            {
                CurrentWriteCharacteristic = writeCharacteristic.Characteristics[CHARACTERISTIC_INDEX];

                CurrentNotifyCharacteristic = notifyCharacteristic.Characteristics[CHARACTERISTIC_INDEX];
                CurrentNotifyCharacteristic.ProtectionLevel = GattProtectionLevel.Plain;
                CurrentNotifyCharacteristic.ValueChanged += currentNotifyCharacteristic_ValueChanged;

                retStatus = await enableNotifications(CurrentNotifyCharacteristic);
            }
            else
            {
                retStatus.status = false;
                retStatus.message = "没有发现特征对象";
            }

            return retStatus;
        }

        /// <summary>
        /// 收到设备的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void currentNotifyCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out byte[] data);
            await UniTask.SwitchToThreadPool();
            OnRecvdData?.Invoke(sender, data);
        }

        /// <summary>
        /// 设置特征对象为接收通知对象
        /// </summary>
        /// <param name="characteristic"></param>
        /// <returns></returns>
        private async Task<RetStatus> enableNotifications(GattCharacteristic characteristic)
        {
            RetStatus retStatus = new RetStatus();

            var result = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(CHARACTERISTIC_NOTIFICATION_TYPE);
            if (result == GattCommunicationStatus.Unreachable)
            {
                retStatus.status = false;
                retStatus.message = "设备不可用";
                return retStatus;
            }
            retStatus.status = true;
            retStatus.message = result.ToString();
            return retStatus;
        }
        #endregion
    }
}

using System;
using System.Windows;

namespace MyEmmControl.Communication
{
    public interface ICommunication
    {
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        void Send(byte[] data);

        /// <summary>
        /// 断开连接
        /// </summary>
        void Dispose();

        /// <summary>
        /// 连接设备与设置
        /// </summary>
        /// <returns></returns>
        bool? ConnectDeviceAndSettingWindow(Window owner);

        /// <summary>
        /// 收到数据
        /// </summary>
        event EventHandler<byte[]> OnRecvdData;

        /// <summary>
        /// Get数据
        /// </summary>
        /// <param name="data"></param>
        /// <remarks>超时时间：5秒</remarks>
        /// <returns></returns>
        byte[] Get(byte[] data);
    }
}

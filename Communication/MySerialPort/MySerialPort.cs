using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyEmmControl.Communication
{
    public class MySerialPort : ICommunication
    {
        public SerialPort serialPort;

        public event EventHandler<byte[]> OnRecvdData;

        public MySerialPort()
        {
            this.serialPort = new SerialPort();
            serialPort.DataReceived += serialPort_DataReceived;
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] result = new byte[serialPort.BytesToRead];

            serialPort.Read(result, 0, serialPort.BytesToRead);

            OnRecvdData?.Invoke(sender, result);
        }

        public bool? ConnectDeviceAndSettingWindow(Window owner)
        {
            var dia =new MySerialPort_ConnectDeviceAndSettingWindow(this);
            dia.Owner = owner;
            return dia.ShowDialog();
        }

        public void Dispose()
        {
            serialPort.Dispose();
        }

        public void Send(byte[] data)
        {
            //获取串口状态，true为已打开，false为未打开
            if (!serialPort.IsOpen)
            {
                Open();
            }

            //发送字节数组
            //参数1：包含要写入端口的数据的字节数组。
            //参数2：参数中从零开始的字节偏移量，从此处开始将字节复制到端口。
            //参数3：要写入的字节数。 
            serialPort.Write(data, 0, data.Length);
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open() => serialPort.Open();

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close() => serialPort.Close();
    }
}

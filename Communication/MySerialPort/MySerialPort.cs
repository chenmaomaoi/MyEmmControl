using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace MyEmmControl.Communication
{
    [Description("串口通信")]
    public class MySerialPort : CommunicationBase
    {
        public SerialPort serialPort;

        public override event EventHandler<byte[]> OnRecvdData;

        private bool getFlag = false;

        private List<byte> _data = new List<byte>();

        public MySerialPort() : this(ChecksumTypes.None) { }

        public MySerialPort(ChecksumTypes checksumType) : base(checksumType)
        {
            this.serialPort = new SerialPort();
            this.ChecksumType = checksumType;
            serialPort.DataReceived += serialPort_DataReceived;
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] result = new byte[serialPort.BytesToRead];
            serialPort.Read(result, 0, serialPort.BytesToRead);

            byte[] dataBody = base.CheckData(result);
            //校验值无效
            if (dataBody.Length <= 0) return;

            if (!getFlag)
            {
                OnRecvdData?.Invoke(sender, result);
            }
            else
            {
                _data.Clear();
                _data.AddRange(result);
            }
        }

        public override bool ConnectDeviceAndSettingWindow(Window owner)
        {
            var dia = new MySerialPort_ConnectDeviceAndSettingWindow(this);
            dia.Owner = owner;
            return (bool)dia.ShowDialog();
        }

        public override void Dispose()
        {
            serialPort.Dispose();
        }

        public override void Send(byte[] data)
        {
            //计算并拼接校验值
            byte[] checkData = _checksumer.Calculate(data);
            byte[] sdata = data.Concat(checkData).ToArray();
            //获取串口状态，true为已打开，false为未打开
            if (!serialPort.IsOpen)
            {
                Open();
            }

            //发送字节数组
            //参数1：包含要写入端口的数据的字节数组。
            //参数2：参数中从零开始的字节偏移量，从此处开始将字节复制到端口。
            //参数3：要写入的字节数。 
            serialPort.Write(sdata, 0, sdata.Length);
        }

        public override byte[] Get(byte[] data)
        {
            getFlag = true;
            Send(data);

            DateTime _sendTime = DateTime.Now;
            while (_data.Count <= 0)
            {
                Thread.Sleep(1);
                if (DateTime.Now.Subtract(_sendTime).TotalSeconds > 5)
                {
                    throw new Exception("串口设备未响应");
                }
            }

            byte[] result = new byte[_data.Count];
            _data.CopyTo(result);
            getFlag = false;
            return result.ToArray();
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

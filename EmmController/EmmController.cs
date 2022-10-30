using MyEmmControl.Attributes;
using MyEmmControl.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Printing;

namespace MyEmmControl
{
    public class EmmController
    {
        /// <summary>
        /// 通信地址
        /// </summary>
        /// <remarks>
        /// <para>0为广播地址</para>
        /// <para>1-247为设备地址</para>
        /// </remarks>
        public byte UARTAddr { get; set; } = 0x01;

        /// <summary>
        /// 指令头
        /// </summary>
        private CommandHead cmdHead { get; set; }

        /// <summary>
        /// 指令体
        /// </summary>
        /// <remarks>读取型指令没有指令体</remarks>
        private byte[] cmdBody { get; set; } = null;

        /// <summary>
        /// 检验字节
        /// </summary>
        /// <remarks>通过校验或者返回固定的0x6B</remarks>
        private byte[] dataCheck { get; set; } = { 0x6B };

        private ICommunication communication;

        private event EventHandler<byte[]> recvdData;

        public EmmController(ICommunication communication)
        {
            this.communication = communication;
            this.communication.OnRecvdData += RecvdData;
        }

        private void RecvdData(object sender, byte[] e)
        {
            if (e.Length > 2 && e[0] == UARTAddr)
            {
                //todo:校验数据


                //掐头去尾
                byte[] _data = new byte[e.Length - 2];
                for (int i = 1; i < e.Length - 1; i++)
                {
                    _data[i - 1] = e[i];
                }
                recvdData?.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="cmdhead"></param>
        /// <param name="cmdbody"></param>
        public void SendCommand(CommandHead cmdhead, byte[] cmdbody = null)
        {
            cmdHead = cmdhead;

            //取命令
            var _headattr = cmdhead.GetAttribute<CommandAttribute>();
            byte[] _head = _headattr.Command;

            //拼接命令
            byte[] _cmd = (cmdbody == null)
                       ? _head.Concat(cmdBody).ToArray()
                       : _head.Concat(cmdBody).Concat(dataCheck).ToArray();


            //根据指令类型绑定返回,并且将返回值装入
            //需要处理指令异常
            //todo:"指令字符串"的二进制版本 https://github.com/SlimeNull/NullLib.CommandLine
            //改造改造!使用反射!
            switch (_headattr.ReturnType)
            {
                case CommandReturnType.Common: recvdData += commonRecv; break;
                case CommandReturnType.State: recvdData += stateRecv; break;
                case CommandReturnType.Puls: recvdData += pulsRecv; break;
                case CommandReturnType.MotorPosition:
                    break;
                case CommandReturnType.PositionError:
                    break;
                case CommandReturnType.Encoder:
                    break;
                case CommandReturnType.SPosition:
                    break;
                default:
                    throw new NotImplementedException();
            }

            communication.Send(_cmd);
        }

        private void pulsRecv(object sender, byte[] e)
        {
            //int32  8*4
            if (e.Length == 4)
            {
                Int32 _result = (Int32)e[3] << 24 |
                                (Int32)e[2] << 16 |
                                (Int32)e[1] << 8 |
                                (Int32)e[0];

                OnComplete?.Invoke(cmdHead, new EventResultArgs() { Success = true, Message = _result.ToString() });
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            recvdData -= pulsRecv;
        }

        private void stateRecv(object sender, byte[] e)
        {
            if (e.Length == 1)
            {
                bool _result = e[0] == CommandReturn.True;
                OnComplete?.Invoke(cmdHead, new EventResultArgs() { Success = _result });
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            recvdData -= stateRecv;
        }

        private void commonRecv(object sender, byte[] e)
        {
            if (e.Length == 1)
            {
                bool _result = e[0] == CommandReturn.CommandOK;
                OnComplete?.Invoke(cmdHead, new EventResultArgs() { Success = _result });
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            recvdData -= commonRecv;
        }

        public event TypedEventHandler<CommandHead, EventResultArgs> OnComplete;
    }

    public class EventResultArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

using MyEmmControl.Attributes;
using MyEmmControl.Communication;
using System;
using System.Linq;
using Windows.Foundation;

namespace MyEmmControl.EmmController
{
    public class Controller
    {
        /// <summary>
        /// 通信地址
        /// </summary>
        /// <remarks>
        /// 0为广播地址<para/>
        /// 1-247为设备地址<para/>
        /// </remarks>
        public byte[] UARTAddr { get; set; } = { 0x01 };

        /// <summary>
        /// 编码器值
        /// </summary>
        public ushort EncoderValue { get; private set; }

        /// <summary>
        /// 脉冲数
        /// </summary>
        public int PulsCount { get; private set; }

        /// <summary>
        /// 电机位置
        /// </summary>
        public int MotorPosition { get; private set; }

        /// <summary>
        /// 电机位置（电机转过的角度）
        /// </summary>
        public double MotorPositionAngle { get => MotorPosition * 360 / 65536; }

        /// <summary>
        /// 位置误差
        /// </summary>
        public short PositionError { get; private set; }

        /// <summary>
        /// 堵转状态
        /// </summary>
        public bool BlockageProtectionState { get; private set; }

        /// <summary>
        /// 单圈上电回零状态
        /// </summary>
        public bool InitiationState { get; private set; }

        /// <summary>
        /// 驱动板状态
        /// </summary>
        public bool BoardIsEnable { get; private set; }

        /// <summary>
        /// 细分步数
        /// </summary>
        public byte Subdivision { get; private set; }

        /// <summary>
        /// 保存的正反转参数
        /// </summary>
        /// <remarks>方向，速度，加速度</remarks>
        public CommandBody RotationMemory { get; private set; }

        /// <summary>
        /// 当前的正反转参数
        /// </summary>
        public CommandBody RotationCurrent { get; private set; }

        /// <summary>
        /// 指令头
        /// </summary>
        private EnumCommandHead cmdHead { get; set; }

        /// <summary>
        /// 指令体
        /// </summary>
        /// <remarks>读取型指令没有指令体</remarks>
        private CommandBody cmdBody { get; set; }

        /// <summary>
        /// 检验字节
        /// </summary>
        /// <remarks>通过校验或者返回固定的0x6B</remarks>
        private byte[] dataCheck { get; set; } = { 0x6B };

        private ICommunication communication;

        private event EventHandler<byte[]> recvdData;

        public Controller(ICommunication communication)
        {
            this.communication = communication;
            this.communication.OnRecvdData += (object sender, byte[] e) =>
                {
                    if (e.Length > 2 && e[0] == UARTAddr[0])
                    {
                        //todo:校验数据
                        //固定结尾0x6B校验
                        if (e[e.Length - 1] != 0x6B) return;

                        //掐头去尾
                        byte[] _data = new byte[e.Length - 2];
                        for (int i = 1; i < e.Length - 1; i++)
                        {
                            _data[i - 1] = e[i];
                        }
                        recvdData?.Invoke(sender, e);
                    }
                };
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="cmdhead"></param>
        /// <param name="cmdbody"></param>
        public void SendCommand(EnumCommandHead cmdhead, CommandBody cmdbody = null)
        {
            cmdHead = cmdhead;
            cmdBody = cmdbody;

            //取命令
            var _headattr = cmdhead.GetAttribute<CommandAttribute>();
            byte[] _head = _headattr.Command;

            //拼接命令
            byte[] _cmd = (cmdBody == null)
                       ? UARTAddr.Concat(_head).Concat(dataCheck).ToArray()
                       : UARTAddr.Concat(_head).Concat(cmdBody.GetCommandBody()).Concat(dataCheck).ToArray();

            //绑定返回处理
            typeof(Controller).GetMethod(cmdhead.ToString())?.Invoke(this, null);

            communication.Send(_cmd);
        }

        /// <summary>
        /// 编码器校准
        /// </summary>
        /// <remarks>什么都不做，等待就行了</remarks>
        private void CalibrationEncoder() => recvdData += commonRecv;

        /// <summary>
        /// 设置当前位置为零点
        /// </summary>
        private void SetInitiationPoint() => recvdData += commonRecv;

        /// <summary>
        /// 解除堵转保护
        /// </summary>
        private void ResetBlockageProtection()
        {
            recvdData += recv;
            void recv(object sender, byte[] e)
            {
                if (e.Length == 1)
                {
                    bool _result = e[0] == EnumCommandReturn.CommandOK;
                    if (_result) BlockageProtectionState = false;
                    OnComplete?.Invoke(cmdHead, new EventResultArgs() { Success = _result });
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
                recvdData -= recv;
            }
        }

        /// <summary>
        /// 读取编码器值
        /// </summary>
        private void ReadEncoderValue()
        {
            recvdData += recv;
            void recv(object sender, byte[] e)
            {
                if (e.Length == 2)
                {
                    UInt16 _result = ((ushort)((UInt16)e[1] << 8 | (UInt16)e[0]));
                    this.EncoderValue = _result;
                    OnComplete?.Invoke(cmdHead, new EventResultArgs() { Success = true, Message = _result.ToString() });
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(cmdHead));
                }
                recvdData -= recv;
            }
        }

        /// <summary>
        /// 读取脉冲数
        /// </summary>
        private void ReadPulsCount() => recvdData += int32Recv;

        /// <summary>
        /// 读取电机实时位置
        /// </summary>
        private void ReadMotorPosition() => recvdData += int32Recv;

        /// <summary>
        /// 读取位置误差
        /// </summary>
        private void ReadPositionError()
        {
            recvdData += recv;
            void recv(object sender, byte[] e)
            {
                if (e.Length == 2)
                {
                    Int16 _result = (short)((Int16)e[1] << 8 | (Int16)e[0]);
                    this.PositionError = _result;
                    OnComplete?.Invoke(cmdHead, new EventResultArgs() { Success = true, Message = _result.ToString() });
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(cmdHead));
                }
                recvdData -= recv;
            }
        }

        /// <summary>
        /// 读取驱动板使能状态
        /// </summary>
        private void IsEnable() => recvdData += stateRecv;

        /// <summary>
        /// 读取堵转状态
        /// </summary>
        private void ReadBlockageProtectionState() => recvdData += stateRecv;

        /// <summary>
        /// 读取单圈上电回零状态
        /// </summary>
        private void ReadInitiationState() => recvdData += stateRecv;

        /// <summary>
        /// 修改细分步数
        /// </summary>
        private void UpdateSubdivision() => recvdData += commonRecv;

        /// <summary>
        /// 修改串口通讯地址
        /// </summary>
        private void UpdateUARTAddr() => recvdData += commonRecv;

        /// <summary>
        /// 使能驱动板
        /// </summary>
        private void Enable() => recvdData += commonRecv;
        private void Disable() => recvdData += commonRecv;

        /// <summary>
        /// 控制电机转动
        /// </summary>
        private void SetRotation() => recvdData += commonRecv;

        /// <summary>
        /// 存储电机正反转参数
        /// </summary>
        private void StoreRotation() => recvdData += commonRecv;

        /// <summary>
        /// 清除电机正反转参数
        /// </summary>
        private void RestoreRotation() => recvdData += commonRecv;

        /// <summary>
        /// 控制电机转动
        /// </summary>
        private void SetPosition()
        {
            recvdData += recv1;

            void recv1(object sender, byte[] e)
            {
                if (e.Length == 1)
                {
                    bool _result = e[0] == EnumCommandReturn.CommandOK;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
                recvdData -= recv1;
                recvdData += recv2;
            }

            void recv2(object sender, byte[] e)
            {
                if (e.Length == 1)
                {
                    bool _result = e[0] == EnumCommandReturn.PcmdRet;

                    OnComplete?.Invoke(cmdHead, new EventResultArgs() { Success = _result, Message = cmdBody.ToString() });
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
                recvdData -= recv2;
            }
        }

        private void int32Recv(object sender, byte[] e)
        {
            if (e.Length == 4)
            {
                Int32 _result = (Int32)e[3] << 24 |
                                (Int32)e[2] << 16 |
                                (Int32)e[1] << 8 |
                                (Int32)e[0];
                switch (cmdHead)
                {
                    case EnumCommandHead.ReadPulsCount: this.PulsCount = _result; break;
                    case EnumCommandHead.ReadMotorPosition: this.MotorPosition = _result; break;

                    default: throw new ArgumentOutOfRangeException(nameof(cmdHead));
                }

                this.PulsCount = _result;
                OnComplete?.Invoke(cmdHead, new EventResultArgs() { Success = true, Message = _result.ToString() });
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            recvdData -= int32Recv;
        }


        private void stateRecv(object sender, byte[] e)
        {
            if (e.Length == 1)
            {
                bool _result = e[0] == EnumCommandReturn.True;
                switch (cmdHead)
                {
                    case EnumCommandHead.IsEnable: this.BoardIsEnable = _result; break;
                    case EnumCommandHead.ReadBlockageProtectionState: this.BlockageProtectionState = _result; break;
                    case EnumCommandHead.ReadInitiationState: this.InitiationState = !_result; break;

                    default: throw new ArgumentOutOfRangeException(nameof(cmdHead));
                }

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
                bool _result = e[0] == EnumCommandReturn.CommandOK;
                byte[] _cmdbody = cmdBody.GetCommandBody();
                switch (cmdHead)
                {
                    case EnumCommandHead.CalibrationEncoder: break;
                    case EnumCommandHead.SetInitiationPoint: break;
                    case EnumCommandHead.UpdateSubdivision: if (_result && _cmdbody.Length == 1) this.Subdivision = _cmdbody[0]; break;
                    case EnumCommandHead.UpdateUARTAddr: if (_result && _cmdbody.Length == 1) this.UARTAddr = new byte[] { _cmdbody[0] }; break;
                    case EnumCommandHead.Enable: if (_result) this.BoardIsEnable = true; break;
                    case EnumCommandHead.Disable: if (_result) this.BoardIsEnable = false; break;
                    case EnumCommandHead.SetRotation: if (_result) this.RotationCurrent = cmdBody; break;
                    case EnumCommandHead.StoreRotation: if (_result) this.RotationMemory = this.RotationCurrent; break;
                    case EnumCommandHead.RestoreRotation: if (_result) this.RotationMemory = null; break;

                    default: throw new ArgumentOutOfRangeException(nameof(cmdHead));
                }

                OnComplete?.Invoke(cmdHead, new EventResultArgs() { Success = _result, Message = cmdBody.ToString() });
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            recvdData -= commonRecv;
        }

        public event TypedEventHandler<EnumCommandHead, EventResultArgs> OnComplete;
    }

    public class EventResultArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

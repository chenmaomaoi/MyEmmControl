using MyEmmControl.Attributes;
using MyEmmControl.Communication;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public byte UARTAddr { get; private set; } = 0x01;

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
        /// 数据校验类型
        /// </summary>
        public ChecksumType ChecksumType { get; private set; }

        /// <summary>
        /// 指令头
        /// </summary>
        private CommandHead _cmdHead { get; set; }

        /// <summary>
        /// 指令体
        /// </summary>
        /// <remarks>读取型指令没有指令体</remarks>
        private CommandBody _cmdBody { get; set; }

        /// <summary>
        /// 检验字节
        /// </summary>
        /// <remarks>通过校验或者返回固定的0x6B</remarks>
        private byte[] _dataCheck { get; set; } = { 0x6B };

        private ICommunication _communication;

        /// <summary>
        /// 位置运动模式结束-到达指定位置
        /// </summary>
        public event EventHandler EventSetPositionDone;

        public Controller(ICommunication communication)
        {
            this._communication = communication;

            //非Get模式收到数据
            this._communication.OnRecvdData += (object sender, byte[] e) =>
                {
                    byte[] dat = DataFilter(e);
                    if (dat.Length == 1 && dat[0] == (byte)CommandReturnValue.PcmdRet)
                    {
                        //角度转动模式结束
                        EventSetPositionDone?.Invoke(sender,new EventArgs());
                    }
                };
        }

        /// <summary>
        /// 数据处理与校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] DataFilter(byte[] data)
        {
            if (data.Length > 2 && data[0] == UARTAddr)
            {
                //todo:校验数据
                //固定结尾0x6B校验
                if (data[data.Length - 1] != 0x6B) return null;

                //掐头去尾
                byte[] ndata = new byte[data.Length - 2];
                for (int i = 1; i < data.Length - 1; i++)
                {
                    ndata[i - 1] = data[i];
                }

                return ndata;
            }
            return null;
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="cmdhead"></param>
        /// <param name="cmdbody"></param>
        public string SendCommand(CommandHead cmdhead, CommandBody cmdbody = null)
        {
            _cmdHead = cmdhead;
            _cmdBody = cmdbody;

            //取命令
            var headattr = _cmdHead.GetAttribute<CommandAttribute>();
            byte[] head = headattr.Command;

            //拼接命令
            byte[] _cmd = (_cmdBody == null)
                       ? new byte[] { UARTAddr }.Concat(head).Concat(_dataCheck).ToArray()
                       : new byte[] { UARTAddr }.Concat(head).Concat(_cmdBody.GetCommandBody()).Concat(_dataCheck).ToArray();

            //倒序
            IEnumerable<byte> uartMessgae = DataFilter(_communication.Get(_cmd)).Reverse();

            //解析返回值
            dynamic retValue = headattr.GetValue(uartMessgae.ToArray());

            //绑定返回处理
            object result = typeof(Controller).GetMethod(_cmdHead.ToString())?.Invoke(this, retValue);
            
            return result.ToString();
        }

        /// <summary>
        /// 编码器校准
        /// </summary>
        /// <remarks>什么都不做，等待就行了</remarks>
        //private bool CalibrationEncoder(byte[] e) => convertRecvCommonAndState(e);

        /// <summary>
        /// 设置当前位置为零点
        /// </summary>
        /// <remarks>什么都不做，等待就行了</remarks>
        //private bool SetInitiationPoint(byte[] e) => convertRecvCommonAndState(e);

        /// <summary>
        /// 解除堵转保护
        /// </summary>
        /// <returns>接触堵转保护成功为true</returns>
        private bool ResetBlockageProtection(byte[] e)
        {
            bool result = convertRecvCommonAndState(e);
            if (result) this.BlockageProtectionState = false;
            return result;
        }

        /// <summary>
        /// 读取编码器值
        /// </summary>
        private ushort ReadEncoderValue(byte[] e)
        {
            if (e.Length != 2) throw new ArgumentOutOfRangeException(nameof(e));
            this.EncoderValue = (ushort)convertRecvInt32(e);
            return this.EncoderValue;
        }

        /// <summary>
        /// 读取脉冲数
        /// </summary>
        private int ReadPulsCount(byte[] e)
        {
            if (e.Length != 4) throw new ArgumentOutOfRangeException(nameof(e));
            this.PulsCount = convertRecvInt32(e);
            return this.PulsCount;
        }

        /// <summary>
        /// 读取电机实时位置
        /// </summary>
        private int ReadMotorPosition(byte[] e)
        {
            if (e.Length != 4) throw new ArgumentOutOfRangeException(nameof(e));
            this.MotorPosition = convertRecvInt32(e);
            return this.MotorPosition;
        }

        /// <summary>
        /// 读取位置误差
        /// </summary>
        private short ReadPositionError(byte[] e)
        {
            if (e.Length != 2) throw new ArgumentOutOfRangeException(nameof(e));
            this.PositionError = convertRecvInt16(e);
            return this.PositionError;
        }

        /// <summary>
        /// 读取驱动板使能状态
        /// </summary>
        private bool IsEnable(byte[] e)
        {
            this.BoardIsEnable = convertRecvCommonAndState(e);
            return this.BoardIsEnable;
        }

        /// <summary>
        /// 读取堵转状态
        /// </summary>
        private bool ReadBlockageProtectionState(byte[] e)
        {
            this.BlockageProtectionState = convertRecvCommonAndState(e);
            return this.BlockageProtectionState;
        }

        /// <summary>
        /// 读取单圈上电回零状态
        /// </summary>
        private bool ReadInitiationState(byte[] e)
        {
            bool result = convertRecvCommonAndState(e);
            this.InitiationState = !result;
            return result;
        }

        /// <summary>
        /// 修改细分步数
        /// </summary>
        private bool UpdateSubdivision(byte[] e)
        {
            bool result = convertRecvCommonAndState(e);
            if (result) this.Subdivision = (byte)_cmdBody.Data;
            return result;
        }

        /// <summary>
        /// 修改串口通讯地址
        /// </summary>
        private bool UpdateUARTAddr(byte[] e)
        {
            bool result = convertRecvCommonAndState(e);
            if (result) this.UARTAddr = (byte)_cmdBody.Data;
            return result;
        }

        /// <summary>
        /// 使能驱动板
        /// </summary>
        private bool Enable(byte[] e)
        {
            bool result = convertRecvCommonAndState(e);
            if (result) this.BoardIsEnable = true;
            return result;
        }
        private bool Disable(byte[] e)
        {
            bool result = convertRecvCommonAndState(e);
            if (result) this.BoardIsEnable = false;
            return result;
        }

        /// <summary>
        /// 控制电机转动
        /// </summary>
        private bool SetRotation(byte[] e)
        {
            bool result = convertRecvCommonAndState(e);
            if (result) this.RotationCurrent = _cmdBody;
            return result;
        }

        /// <summary>
        /// 存储电机正反转参数
        /// </summary>
        private bool StoreRotation(byte[] e)
        {
            bool result = convertRecvCommonAndState(e);
            if (result) this.RotationMemory = this.RotationCurrent;
            return result;
        }

        /// <summary>
        /// 清除电机正反转参数
        /// </summary>
        private bool RestoreRotation(byte[] e)
        {
            bool result = convertRecvCommonAndState(e);
            if (result) this.RotationMemory = null;
            return result;
        }

        /// <summary>
        /// 控制电机转动
        /// </summary>
        //private bool SetPosition(byte[] e) => convertRecvCommonAndState(e);

        private short convertRecvInt16(byte[] e)
        {
            if (e.Length != 2) throw new ArgumentOutOfRangeException(nameof(e));
            return BitConverter.ToInt16(e, 0);
        }

        private int convertRecvInt32(byte[] e)
        {
            return BitConverter.ToInt32(e, 0);
        }

        private bool convertRecvCommonAndState(byte[] e)
        {
            if (e.Length != 1) throw new ArgumentOutOfRangeException(nameof(e));
            return (e[0] == (byte)CommandReturnValue.CommandOK) || (e[0] == (byte)CommandReturnValue.True);
        }
    }
}

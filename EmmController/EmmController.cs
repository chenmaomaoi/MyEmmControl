using MyEmmControl.Extensions;
using MyEmmControl.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MyEmmControl
{
    public class EmmController
    {
        /// <summary>
        /// 通信地址
        /// </summary>
        /// <remarks>
        /// 0为广播地址<para/>
        /// 1-247为设备地址<para/>
        /// </remarks>
        [Description(nameof(CommandHeads.UpdateUARTAddr))]
        public byte UARTAddr { get; set; } = 0x01;

        /// <summary>
        /// 编码器值
        /// </summary>
        [Description(nameof(CommandHeads.ReadEncoderValue))]
        public ushort EncoderValue { get; private set; }

        /// <summary>
        /// 脉冲数
        /// </summary>
        [Description(nameof(CommandHeads.ReadPulsCount))]
        public int PulsCount { get; private set; }

        /// <summary>
        /// 电机位置
        /// </summary>
        [Description(nameof(CommandHeads.ReadMotorPosition))]
        public int MotorPosition { get; private set; }

        /// <summary>
        /// 电机位置（电机转过的角度）
        /// </summary>
        public double MotorPositionAngle { get => MotorPosition * 360 / 65536; }

        /// <summary>
        /// 位置误差
        /// </summary>
        [Description(nameof(CommandHeads.ReadPositionError))]
        public short PositionError { get; private set; }

        /// <summary>
        /// 堵转状态
        /// </summary>
        [Description(nameof(CommandHeads.ReadBlockageProtectionState))]
        public bool BlockageProtectionState { get; private set; }

        /// <summary>
        /// 单圈上电回零状态
        /// </summary>
        [Description(nameof(CommandHeads.ReadInitiationState))]
        public bool InitiationState
        {
            get => _InitiationState;
            private set => _InitiationState = !value;
        }
        private bool _InitiationState;

        /// <summary>
        /// 驱动板状态
        /// </summary>
        [Description(nameof(CommandHeads.IsEnable))]
        public bool BoardIsEnable { get; private set; }

        /// <summary>
        /// 细分步数
        /// </summary>
        [Description(nameof(CommandHeads.UpdateSubdivision))]
        public byte Subdivision { get; private set; }

        /// <summary>
        /// 保存的正反转参数
        /// </summary>
        /// <remarks>方向，速度，加速度</remarks>
        [Description(nameof(CommandHeads.StoreRotation))]
        public CommandBody RotationMemory { get; private set; }

        /// <summary>
        /// 当前的正反转参数
        /// </summary>
        [Description(nameof(CommandHeads.SetRotation))]
        public CommandBody RotationCurrent { get; private set; }

        /// <summary>
        /// 数据校验类型
        /// </summary>
        public ChecksumTypes ChecksumType { get; set; }

        /// <summary>
        /// 指令头
        /// </summary>
        private CommandHeads _cmdHead { get; set; }

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

        public EmmController(ICommunication communication)
        {
            this._communication = communication;

            //非Get模式收到数据
            this._communication.OnRecvdData += (object sender, byte[] e) =>
            {
                byte[] dat = DataFilter(e);
                if (dat.Length == 1 && dat[0] == (byte)CommandReturnValues.PcmdRet)
                {
                    //角度转动模式结束
                    EventSetPositionDone?.Invoke(sender, new EventArgs());
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
                //todo:校验已移动至单独类处理
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
        public string SendCommand(CommandHeads cmdhead, CommandBody cmdbody = null)
        {
            _cmdHead = cmdhead;
            _cmdBody = cmdbody;

            //取命令
            var headattr = _cmdHead.GetFieldAttribute<CommandAttribute>();
            byte[] head = headattr.Command;

            //拼接命令
            byte[] _cmd = (_cmdBody == null)
                       ? new byte[] { UARTAddr }.Concat(head).Concat(_dataCheck).ToArray()
                       : new byte[] { UARTAddr }.Concat(head).Concat(_cmdBody.GetCommandBody()).Concat(_dataCheck).ToArray();

            //发送命令并处理校验返回值，倒序
            IEnumerable<byte> uartMessgae = DataFilter(_communication.Get(_cmd)).Reverse();

            //解析返回值
            dynamic retValue = headattr.GetValue(uartMessgae.ToArray());

            //根据返回值处理标签进行操作
            switch (headattr.ReturnOperate)
            {
                case CommandReturnOperationTypes.Value:
                    PropertyInfo propertyInfo = typeof(EmmController).GetProperty(_cmdHead.ToString());
                    if (propertyInfo == null) throw new NotImplementedException(_cmdHead.ToString());
                    propertyInfo.SetValue(this, retValue);
                    break;
                case CommandReturnOperationTypes.Other:
                    MethodInfo methodInfo = typeof(EmmController).GetMethod(_cmdHead.ToString());
                    if (methodInfo == null) throw new NotImplementedException(_cmdHead.ToString());
                    methodInfo.Invoke(this, retValue);
                    break;
                case CommandReturnOperationTypes.No_Operation: break;
                default: throw new ArgumentOutOfRangeException(nameof(headattr.ReturnOperate));
            }
            return retValue;
        }

        /// <summary>
        /// 解除堵转保护
        /// </summary>
        /// <returns>接触堵转保护成功为true</returns>
        private bool ResetBlockageProtection(bool e)
        {
            if (e) this.BlockageProtectionState = false;
            return e;
        }

        /// <summary>
        /// 修改细分步数
        /// </summary>
        private bool UpdateSubdivision(bool e)
        {
            if (e) this.Subdivision = (byte)_cmdBody.Data;
            return e;
        }

        /// <summary>
        /// 修改串口通讯地址
        /// </summary>
        private bool UpdateUARTAddr(bool e)
        {
            if (e) this.UARTAddr = (byte)_cmdBody.Data;
            return e;
        }

        /// <summary>
        /// 使能驱动板
        /// </summary>
        private bool Enable(bool e)
        {
            if (e) this.BoardIsEnable = true;
            return e;
        }
        private bool Disable(bool e)
        {
            if (e) this.BoardIsEnable = false;
            return e;
        }

        /// <summary>
        /// 控制电机转动
        /// </summary>
        private bool SetRotation(bool e)
        {
            if (e) this.RotationCurrent = _cmdBody;
            return e;
        }

        /// <summary>
        /// 存储电机正反转参数
        /// </summary>
        private bool StoreRotation(bool e)
        {
            if (e) this.RotationMemory = this.RotationCurrent;
            return e;
        }

        /// <summary>
        /// 清除电机正反转参数
        /// </summary>
        private bool RestoreRotation(bool e)
        {
            if (e) this.RotationMemory = null;
            return e;
        }
    }
}

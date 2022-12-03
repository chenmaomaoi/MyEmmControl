using MyEmmControl.Extensions;
using MyEmmControl.Communication;
using System;
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
        [Description(nameof(EmmCmdHeads.UpdateUARTAddr))]
        public byte UARTAddr { get; set; } = 0x01;

        /// <summary>
        /// 编码器值
        /// </summary>
        [Description(nameof(EmmCmdHeads.EncoderValue))]
        public ushort EncoderValue { get; private set; } = default;

        /// <summary>
        /// 脉冲数
        /// </summary>
        [Description(nameof(EmmCmdHeads.PulsCount))]
        public int PulsCount { get; private set; } = default;

        /// <summary>
        /// 电机位置
        /// </summary>
        [Description(nameof(EmmCmdHeads.MotorPosition))]
        public int MotorPosition { get; private set; } = default;

        /// <summary>
        /// 电机位置（电机转过的角度）
        /// </summary>
        public double MotorPositionAngle { get => MotorPosition * 360 / 65536d; }

        /// <summary>
        /// 位置误差
        /// </summary>
        [Description(nameof(EmmCmdHeads.PositionError))]
        public short PositionError { get; private set; } = default;

        /// <summary>
        /// 堵转状态
        /// </summary>
        [Description(nameof(EmmCmdHeads.BlockageProtectionState))]
        public bool BlockageProtectionState { get; private set; } = default;

        /// <summary>
        /// 单圈上电回零状态
        /// </summary>
        [Description(nameof(EmmCmdHeads.InitiationState))]
        public bool InitiationState { get; private set; } = default;

        /// <summary>
        /// 驱动板状态
        /// </summary>
        [Description(nameof(EmmCmdHeads.BoardIsEnable))]
        public bool BoardIsEnable { get; private set; } = default;

        /// <summary>
        /// 细分步数
        /// </summary>
        [Description(nameof(EmmCmdHeads.UpdateSubdivision))]
        public byte Subdivision { get; private set; } = default;

        /// <summary>
        /// 保存的正反转参数
        /// </summary>
        /// <remarks>方向，速度，加速度</remarks>
        [Description(nameof(EmmCmdHeads.StoreRotation))]
        public EmmCmdBody RotationMemory { get; private set; }

        /// <summary>
        /// 当前的正反转参数
        /// </summary>
        [Description(nameof(EmmCmdHeads.SetRotation))]
        public EmmCmdBody RotationCurrent { get; private set; }

        /// <summary>
        /// 指令头
        /// </summary>
        private EmmCmdHeads emmCmdHead { get; set; }

        /// <summary>
        /// 指令体
        /// </summary>
        /// <remarks>读取型指令没有指令体</remarks>
        private EmmCmdBody emmCmdBody { get; set; }

        public ICommunication Communication { get; private set; }

        /// <summary>
        /// 位置运动模式结束-到达指定位置
        /// </summary>
        public event EventHandler EventSetPositionDone;

        public EmmController(ICommunication communication)
        {
            this.Communication = communication;

            //todo:AOP
            //非Get模式收到数据
            this.Communication.OnRecvdData += (object sender, byte[] e) =>
            {
                byte[] dat = DataFilter(e);
                if (dat.Length == 1 && dat[0] == (byte)EmmCmdReturnValues.PcmdRet)
                {
                    //角度转动模式结束
                    EventSetPositionDone?.Invoke(sender, new EventArgs());
                }
                else
                {
                    //todo:记录抛出的数据
                    throw new NotImplementedException(dat.ToString());
                }
            };
        }

        /// <summary>
        /// 数据处理：去除前缀地址
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] DataFilter(byte[] data)
        {
            //todo:AOP
            if (data.Length > 1 && data[0] == UARTAddr)
            {
                //抛弃通信地址
                return data.Skip(1).ToArray();
            }
            return null;
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="emmCmdhead"></param>
        /// <param name="emmCmdbody"></param>
        public string SendCommand(EmmCmdHeads emmCmdhead, EmmCmdBody emmCmdbody = null)
        {
            emmCmdHead = emmCmdhead;
            emmCmdBody = emmCmdbody;

            //取命令
            var headattr = emmCmdHead.GetFieldAttribute<EmmCmdAttribute>();
            byte[] head = headattr.EmmCmd;

            //拼接命令
            byte[] _cmd = (emmCmdBody == null)
                       ? new byte[] { UARTAddr }.Concat(head).ToArray()
                       : new byte[] { UARTAddr }.Concat(head).Concat(emmCmdBody.GetCommandBody()).ToArray();

            //发送命令并处理校验返回值，倒序
            var result = Communication.Get(_cmd);

            var uartMessgae = DataFilter(result.ToArray());

            //解析返回值
            var retValue = headattr.GetValue(uartMessgae);

            //根据返回值处理标签进行操作
            switch (headattr.ReturnOperate)
            {
                case EmmCmdReturnOperationTypes.Value:
                    PropertyInfo propertyInfo = this.GetType().GetProperty(emmCmdHead.ToString());
                    propertyInfo?.SetValue(this, retValue);
                    break;
                case EmmCmdReturnOperationTypes.Other:
                    MethodInfo methodInfo = this.GetType().GetMethod(emmCmdHead.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);
                    methodInfo?.Invoke(this, new object[] { retValue });
                    break;
                case EmmCmdReturnOperationTypes.No_Operation: break;
                default: throw new ArgumentOutOfRangeException(nameof(headattr.ReturnOperate));
            }
            return retValue.ToString();
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
            if (e) this.Subdivision = (byte)emmCmdBody.Data;
            return e;
        }

        /// <summary>
        /// 修改串口通讯地址
        /// </summary>
        private bool UpdateUARTAddr(bool e)
        {
            if (e) this.UARTAddr = (byte)emmCmdBody.Data;
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
            if (e) this.RotationCurrent = emmCmdBody;
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

using MyEmmControl.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEmmControl
{
    /// <summary>
    /// 枚举了返回值
    /// </summary>
    public static class CommandReturn
    {
        /// <summary>
        /// 收到指令并且指令正确
        /// </summary>
        public static readonly byte CommandOK = 0x02;

        /// <summary>
        /// 指令有误
        /// </summary>
        public static readonly byte CommandError = 0xEE;

        /// <summary>
        /// 位置模式-指令执行完毕
        /// </summary>
        public static readonly byte PcmdRet = 0x9F;

        public static readonly byte True = 0x01;
        public static readonly byte False = 0x00;
    }

    //变为静态类
    //在类体里实现返回值的转换
    public enum CommandReturnType
    {
        /// <summary>
        /// 一般返回=>OK Error
        /// </summary>
        Common,

        /// <summary>
        /// 状态报告
        /// </summary>
        State,

        /// <summary>
        /// 脉冲数量报告
        /// </summary>
        Puls,

        /// <summary>
        /// 电机位置报告
        /// </summary>
        MotorPosition,

        /// <summary>
        /// 位置误差报告
        /// </summary>
        PositionError,

        /// <summary>
        /// 编码器数值报告
        /// </summary>
        Encoder,

        /// <summary>
        /// 位置模式控制电机转动专用返回值
        /// </summary>
        SPosition
    }
}

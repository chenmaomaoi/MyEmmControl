using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MyEmmControl
{
    /// <summary>
    /// 命令体所包含的信息
    /// </summary>
    public class EmmCmdBody
    {
        /// <summary>
        /// 单字节，用于修改串口通信地址和细分步数。
        /// </summary>
        public byte? Data { get; set; }

        /// <summary>
        /// 转动方向
        /// </summary>
        public DirectionOfRotation? Direction { get; set; }

        /// <summary>
        /// 转动速度
        /// </summary>
        public UInt16 Speed { get; set; }

        /// <summary>
        /// 加速度
        /// </summary>
        public byte Acceleration { get; set; }

        /// <summary>
        /// 脉冲数
        /// </summary>
        public UInt32? PulsTimes { get; set; }

        /// <summary>
        /// 生成
        /// </summary>
        public byte[] GetCommandBody()
        {
            IEnumerable<byte> result;
            if (Direction == null) return new byte[] { (byte)Data };

            //拼接方向 速度
            UInt16 _DS = (ushort)((UInt16)Direction | Speed);
            IEnumerable<byte> _bDS = BitConverter.GetBytes(_DS).Reverse();
            //拼接加速度
            result = _bDS.Concat(new byte[] { Acceleration });

            if (PulsTimes != null)
            {
                //拼接脉冲数
                result = result.Concat(BitConverter.GetBytes((UInt32)PulsTimes).Reverse());
            }
            return result.ToArray();
        }

        public override string ToString()
        {
            return GetCommandBody().ToString();
        }
    }

    /// <summary>
    /// 电机转动方向
    /// </summary>
    public enum DirectionOfRotation : ushort
    {
        /// <summary>
        ///顺时针 clockwise
        /// </summary>
        [Description("顺时针")]
        CW = 0x0000,

        /// <summary>
        /// 逆时针 counterclockwise
        /// </summary>
        [Description("逆时针")]
        CCW = 0x1000
    }
}

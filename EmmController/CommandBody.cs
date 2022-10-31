using System;
using System.Collections.Generic;
using System.Linq;

namespace MyEmmControl
{
    public class CommandBody
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
            byte[] result;
            if (Direction == null) return new byte[] { (byte)Data };

            //拼接方向 速度
            UInt16 _DS = (ushort)((UInt16)Direction | Speed);
            IEnumerable<byte> _bDS = (IEnumerable<byte>)Convert.ChangeType(_DS, typeof(byte[]));
            //拼接加速度
            IEnumerable<byte> _bDSA = _bDS.Concat(new byte[] { Acceleration });

            if (PulsTimes == null)
            {
                result = (byte[])_bDSA;

            }
            else
            {
                //拼接脉冲数
                IEnumerable<byte> _bDSAP = _bDSA.Concat((IEnumerable<byte>)Convert.ChangeType(PulsTimes, typeof(byte[])));
                result = (byte[])_bDSAP;
            }
            return result;
        }

        public override string ToString()
        {
            return GetCommandBody().ToString();
        }
    }

    public enum DirectionOfRotation : UInt16
    {
        /// <summary>
        ///顺时针 clockwise
        /// </summary>
        CW = 0x0000,

        /// <summary>
        /// 逆时针 counterclockwise
        /// </summary>
        CCW = 0x1000
    }
}

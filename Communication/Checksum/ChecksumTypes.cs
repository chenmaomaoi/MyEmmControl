using MyEmmControl.Communication.Checksum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyEmmControl.Communication
{
    /// <summary>
    /// 校验类型
    /// </summary>
    public enum ChecksumTypes
    {

        /// <summary>
        /// 固定值0x6B校验
        /// </summary>
        [Description("0x6B")]
        [ChecksumDataLength(1)]
        Fixed_0x6B,

        /// <summary>
        /// CRC校验
        /// </summary>
        [Description("CRC-8")]
        [ChecksumDataLength(1)]
        CRC_8,

        /// <summary>
        /// 无校验
        /// </summary>
        [Description("无校验")]
        [ChecksumDataLength(0)]
        None,
    }
}

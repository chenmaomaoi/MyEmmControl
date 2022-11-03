using MyEmmControl.Communication.Checksum;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 无校验
        /// </summary>
        [ChecksumDataLength(0)] None,

        /// <summary>
        /// 固定值0x6B校验
        /// </summary>
        [ChecksumDataLength(1)] Fixed_0x6B,

        /// <summary>
        /// CRC校验
        /// </summary>
        [ChecksumDataLength(1)] CRC_8
    }
}

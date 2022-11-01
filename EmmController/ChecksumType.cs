using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEmmControl.EmmController
{
    public enum ChecksumType
    {
        /// <summary>
        /// 固定值0x6B校验
        /// </summary>
        Fixed_0x6B,

        /// <summary>
        /// CRC校验
        /// </summary>
        CRC
    }
}

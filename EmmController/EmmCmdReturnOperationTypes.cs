using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEmmControl
{
    /// <summary>
    /// 控制命令返回之后执行的操作
    /// </summary>
    public enum EmmCmdReturnOperationTypes
    {
        /// <summary>
        /// 无额外操作，直接丢弃
        /// </summary>
        No_Operation,

        /// <summary>
        /// 值
        /// </summary>
        Value,

        /// <summary>
        /// 其余特殊操作
        /// </summary>
        Other
    }
}

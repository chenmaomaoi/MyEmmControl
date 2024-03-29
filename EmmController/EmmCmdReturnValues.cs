﻿namespace MyEmmControl
{
    /// <summary>
    /// 命令返回值（常量）
    /// </summary>
    public enum EmmCmdReturnValues : byte
    {
        /// <summary>
        /// 收到指令并且指令正确
        /// </summary>
        OK = 0x02,

        /// <summary>
        /// 指令有误
        /// </summary>
        Error = 0xEE,

        /// <summary>
        /// 位置模式-指令执行完毕
        /// </summary>
        PcmdRet = 0x9F,

        True = 0x01,
        False = 0x00
    }
}

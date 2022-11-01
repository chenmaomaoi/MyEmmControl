namespace MyEmmControl.EmmController
{
    /// <summary>
    /// 返回值
    /// </summary>
    public enum CommandReturnValue : byte
    {
        /// <summary>
        /// 收到指令并且指令正确
        /// </summary>
        CommandOK = 0x02,

        /// <summary>
        /// 指令有误
        /// </summary>
        CommandError = 0xEE,

        /// <summary>
        /// 位置模式-指令执行完毕
        /// </summary>
        PcmdRet = 0x9F,

        True = 0x01,
        False = 0x00
    }

    /// <summary>
    /// 返回的值类型
    /// </summary>
    public enum CommandReturnValueType
    {
        Bool,
        State,
        UInt16,
        Int16,
        Int32
    }
}

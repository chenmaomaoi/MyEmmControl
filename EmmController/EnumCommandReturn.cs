namespace MyEmmControl.EmmController
{
    /// <summary>
    /// 枚举了返回值
    /// </summary>
    public static class EnumCommandReturn
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
}

using System.ComponentModel;

namespace MyEmmControl
{
    /// <summary>
    /// 命令头
    /// </summary>
    public enum EmmCmdHeads
    {
        #region 触发动作命令
        /// <summary>
        /// 触发编码器校准
        /// </summary>
        /// <remarks>无额外参数,通用返回</remarks>
        [Description("触发编码器校准")]
        [EmmCmd(new byte[] { 0x06, 0x45 }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.No_Operation)]
        CalibrationEncoder,

        /// <summary>
        /// 设置当前位置为零点
        /// </summary>
        /// <remarks>无额外参数,通用返回</remarks>
        [Description("设置当前位置为零点")]
        [EmmCmd(new byte[] { 0x0A, 0X6D }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.No_Operation)]
        SetInitiationPoint,

        /// <summary>
        /// 解除堵转保护
        /// </summary>
        /// <remarks>无额外参数,通用返回</remarks>
        [Description("解除堵转保护")]
        [EmmCmd(new byte[] { 0x0E, 0x52 }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.Other)]
        ResetBlockageProtection,
        #endregion

        #region 读取参数命令
        /// <summary>
        /// 读取编码器值
        /// </summary>
        /// <remarks>无额外参数,返回编码器数值 uint16</remarks>
        [Description("读取编码器值")]
        [EmmCmd(new byte[] { 0x30 }, EmmCmdReturnValueTypes.UInt16, EmmCmdReturnOperationTypes.Value)]
        ReadEncoderValue,

        /// <summary>
        /// 读取脉冲数
        /// </summary>
        /// <remarks>无额外参数,返回脉冲数 int32</remarks>
        [Description("读取脉冲数")]
        [EmmCmd(new byte[] { 0x33 }, EmmCmdReturnValueTypes.Int32, EmmCmdReturnOperationTypes.Value)]
        ReadPulsCount,

        /// <summary>
        /// 读取电机实时位置
        /// </summary>
        /// <remarks>无额外参数,返回电机位置 int32</remarks>
        [Description("读取电机实时位置")]
        [EmmCmd(new byte[] { 0x36 }, EmmCmdReturnValueTypes.Int32, EmmCmdReturnOperationTypes.Value)]
        ReadMotorPosition,

        /// <summary>
        /// 读取位置误差
        /// </summary>
        /// <remarks>无额外参数,返回位置误差 int16</remarks>
        [Description("读取位置误差")]
        [EmmCmd(new byte[] { 0x36 }, EmmCmdReturnValueTypes.Int16, EmmCmdReturnOperationTypes.Value)]
        ReadPositionError,

        /// <summary>
        /// 读取驱动板使能状态
        /// </summary>
        /// <remarks>无额外参数,返回状态信息 byte=>bool</remarks>
        [Description("读取驱动板使能状态")]
        [EmmCmd(new byte[] { 0x3A }, EmmCmdReturnValueTypes.State, EmmCmdReturnOperationTypes.Value)]
        IsEnable,

        /// <summary>
        /// 读取堵转状态
        /// </summary>
        /// <remarks>
        /// 无额外参数,返回状态信息 byte=>bool <para/>
        /// false:未发生堵转 <para/>
        /// true:堵转
        /// </remarks>
        [Description("读取堵转状态")]
        [EmmCmd(new byte[] { 0x3E }, EmmCmdReturnValueTypes.State, EmmCmdReturnOperationTypes.Value)]
        ReadBlockageProtectionState,

        /// <summary>
        /// 读取单圈上电回零状态(失败为true)
        /// </summary>
        /// <remarks>
        /// 无额外参数,返回状态信息 byte=>bool <para/>
        /// false:回零正常 <para/>
        /// true:回零失败
        /// </remarks>
        [Description("读取单圈上电回零状态")]
        [EmmCmd(new byte[] { 0x3A }, EmmCmdReturnValueTypes.State, EmmCmdReturnOperationTypes.Value)]
        ReadInitiationState,
        #endregion

        #region 修改参数命令
        /// <summary>
        /// 修改细分步数
        /// </summary>
        /// <remarks>
        /// byte[1] 细分参数 <para/>
        /// 00=>256细分<para/>
        /// 01=>1细分<para/>
        /// FF=>255细分<para/>
        /// 通用返回
        /// </remarks>
        [Description("修改细分步数")]
        [EmmCmd(new byte[] { 0x84 }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.Other)]
        UpdateSubdivision,

        /// <summary>
        /// 修改串口通讯地址
        /// </summary>
        /// <remarks>
        /// byte[1] 通讯地址 <para/>
        /// 可用值1-247(0xF7)<para/>
        /// 通用返回
        /// </remarks>
        [Description("修改串口通讯地址")]
        [EmmCmd(new byte[] { 0xAE }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.Other)]
        UpdateUARTAddr,
        #endregion

        #region 运动控制命令
        /// <summary>
        /// 使能驱动板
        /// </summary>
        /// <remarks>无额外参数,通用返回</remarks>
        [Description("使能驱动板")]
        [EmmCmd(new byte[] { 0xF3, 0x01 }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.Other)]
        Enable,

        [Description("关闭驱动板")]
        [EmmCmd(new byte[] { 0xF3, 0x00 }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.Other)]
        Disable,

        /// <summary>
        /// 控制电机转动
        /// </summary>
        /// <remarks>
        /// 速度模式<para/>
        /// 电机将一直以设置的速度转动下去<para/>
        /// byte[2] 方向和速度<para/>
        /// --最高半字节0x0=>顺时针 0x1=>逆时针<para/>
        /// --0x4FF => 速度挡位<para/>
        /// byte[1] 加速度<para/>
        /// 通用返回
        /// </remarks>
        [Description("控制电机转动-速度模式")]
        [EmmCmd(new byte[] { 0xF6 }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.Other)]
        SetRotation,

        /// <summary>
        /// 存储电机正反转参数
        /// </summary>
        /// <remarks>
        /// 电机上电之后将根据保存的速度控制模式转动 <para/>
        /// 无额外参数,通用返回
        /// </remarks>
        [Description("存储转动参数")]
        [EmmCmd(new byte[] { 0xFF, 0xC8 }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.Other)]
        StoreRotation,

        /// <summary>
        /// 清除电机正反转参数
        /// </summary>
        /// <remarks>无额外参数,通用返回</remarks>
        [Description("清除转动参数")]
        [EmmCmd(new byte[] { 0xFF, 0xCA }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.Other)]
        RestoreRotation,

        /// <summary>
        /// 控制电机转动
        /// </summary>
        /// <remarks>
        /// 位置模式<para/>
        /// 电机转动到设定位置后停止<para/>
        /// byte[2] 方向和速度<para/>
        /// --最高半字节0x0=>顺时针 0x1=>逆时针<para/>
        /// --0x4FF => 速度挡位<para/>
        /// byte[1] 加速度<para/>
        /// byte[3] 脉冲数<para/>
        /// 指令和返回需要特殊处理
        /// </remarks>
        [Description("控制电机转动-位置模式")]
        [EmmCmd(new byte[] { 0xFD }, EmmCmdReturnValueTypes.Bool, EmmCmdReturnOperationTypes.No_Operation)]
        SetPosition
        #endregion
    }
}

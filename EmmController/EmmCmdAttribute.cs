using System;
using System.Reflection.Emit;

namespace MyEmmControl
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EmmCmdAttribute : Attribute
    {
        public byte[] EmmCmd { get; set; }

        public EmmCmdReturnValueTypes ReturnValueType { get; set; }

        public EmmCmdReturnOperationTypes ReturnOperate { get; set; }

        public EmmCmdAttribute(byte[] emmCmd, EmmCmdReturnValueTypes returnValueType, EmmCmdReturnOperationTypes returnOperate)
        {
            EmmCmd = emmCmd;
            ReturnValueType = returnValueType;
            ReturnOperate = returnOperate;
        }

        public dynamic GetValue(byte[] e)
        {
            switch (ReturnValueType)
            {
                case EmmCmdReturnValueTypes.Bool:
                case EmmCmdReturnValueTypes.State:
                    if (e.Length != 1) throw new ArgumentOutOfRangeException(nameof(e));
                    return (e[0] == (byte)EmmCmdReturnValues.OK) || (e[0] == (byte)EmmCmdReturnValues.True);
                case EmmCmdReturnValueTypes.Int16: return BitConverter.ToInt16(e, 0);
                case EmmCmdReturnValueTypes.UInt16: return BitConverter.ToUInt16(e, 0);
                case EmmCmdReturnValueTypes.Int32: return BitConverter.ToInt32(e, 0);
                default: throw new ArgumentOutOfRangeException(nameof(e));
            }
        }
    }
}

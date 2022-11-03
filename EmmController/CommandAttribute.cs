using System;
using System.Reflection.Emit;

namespace MyEmmControl
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandAttribute : Attribute
    {
        public byte[] Command { get; set; }

        public CommandReturnValueTypes ReturnValueType { get; set; }

        public CommandReturnOperationTypes ReturnOperate { get; set; }

        public CommandAttribute(byte[] command, CommandReturnValueTypes returnValueType, CommandReturnOperationTypes returnOperate)
        {
            Command = command;
            ReturnValueType = returnValueType;
            ReturnOperate = returnOperate;
        }

        public dynamic GetValue(byte[] e)
        {
            switch (ReturnValueType)
            {
                case CommandReturnValueTypes.Bool:
                case CommandReturnValueTypes.State:
                    if (e.Length != 1) throw new ArgumentOutOfRangeException(nameof(e));
                    return (e[0] == (byte)CommandReturnValues.CommandOK) || (e[0] == (byte)CommandReturnValues.True);
                case CommandReturnValueTypes.Int16: return BitConverter.ToInt16(e, 0);
                case CommandReturnValueTypes.UInt16: return BitConverter.ToUInt16(e, 0);
                case CommandReturnValueTypes.Int32: return BitConverter.ToInt32(e, 0);
                default: throw new ArgumentOutOfRangeException(nameof(e));
            }
        }
    }
}

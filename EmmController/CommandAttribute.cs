using MyEmmControl.EmmController;
using System;
using System.Reflection.Emit;

namespace MyEmmControl
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandAttribute : Attribute
    {
        public byte[] Command { get; set; }

        public CommandReturnValueType ReturnValueType { get; set; }

        public CommandAttribute(byte[] command, CommandReturnValueType returnValueType)
        {
            Command = command;
            ReturnValueType = returnValueType;
        }

        public dynamic GetValue(byte[] e)
        {
            switch (ReturnValueType)
            {
                case CommandReturnValueType.Bool:
                case CommandReturnValueType.State:
                    if (e.Length != 1)
                        throw new ArgumentOutOfRangeException(nameof(e));
                    return (e[0] == (byte)CommandReturnValue.CommandOK) || (e[0] == (byte)CommandReturnValue.True);
                case CommandReturnValueType.Int16: return BitConverter.ToInt16(e, 0);
                case CommandReturnValueType.UInt16: return BitConverter.ToUInt16(e, 0);
                case CommandReturnValueType.Int32: return BitConverter.ToInt32(e, 0);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}

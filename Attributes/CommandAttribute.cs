using System;

namespace MyEmmControl.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(byte[] command)
        {
            Command = command;
        }

        public byte[] Command { get; set; }
    }
}

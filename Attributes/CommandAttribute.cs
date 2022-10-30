using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEmmControl.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(CommandHeadType headType, CommandReturnType returnType, byte[] command)
        {
            HeadType = headType;
            ReturnType = returnType;
            Command = command;
        }

        public CommandHeadType HeadType { get; set; }

        public CommandReturnType ReturnType { get; set; }

        public byte[] Command { get; set; }
    }
}

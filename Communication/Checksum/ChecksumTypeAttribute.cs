using System;

namespace MyEmmControl.Communication.Checksum
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ChecksumDataLengthAttribute : Attribute
    {
        /// <summary>
        /// 检验数据的长度(byte)
        /// </summary>
        /// <param name="length"></param>
        public ChecksumDataLengthAttribute(int length)
        {
            Length = length;
        }

        /// <summary>
        /// 检验数据的长度(byte)
        /// </summary>
        public int Length { get; set; }

    }
}

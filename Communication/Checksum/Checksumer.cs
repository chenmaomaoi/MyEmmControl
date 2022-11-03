using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Globalization;

namespace MyEmmControl.Communication
{
    /// <summary>
    /// 用于数据校验
    /// </summary>
    public class Checksumer
    {
        public ChecksumTypes ChecksumType { get; set; }

        public Checksumer(ChecksumTypes checksumTypes)
        {
            ChecksumType = checksumTypes;
        }

        /// <summary>
        /// 计算校验值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Calculate(byte[] data) => Calculate(data, data.Length);

        /// <summary>
        /// 计算校验值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] Calculate(byte[] data, int length)
        {
            //拦截None
            if (ChecksumType == ChecksumTypes.None) return null;

            //拦截Fixed
            if (data.Length <= 0) throw new ArgumentNullException(nameof(data));
            string checksumTypeStr = ChecksumType.ToString();
            if (checksumTypeStr.Contains("Fixed")) return new byte[] { GetFixedChecksumValue(checksumTypeStr) };

            //处理其他校验方式
            MethodInfo methodInfo = typeof(Checksumer).GetMethod(checksumTypeStr);
            if (methodInfo == null) throw new NotImplementedException(nameof(ChecksumType));
            object resultObj = methodInfo.Invoke(this, new object[] { data, length });

            //转换最终结果为byte[]
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, resultObj);
                return ms.GetBuffer();
            }
        }

        private byte GetFixedChecksumValue(string checksumTypeStr)
        {
            string fixedStr = checksumTypeStr.Split('_')[1];
            byte res = byte.Parse(fixedStr, NumberStyles.HexNumber);
            return res;
        }

        public bool Check(byte[] data, byte[] check) => Check(data, 0, data.Length, check);

        public bool Check(byte[] data, int startIndex, int endIndex, byte[] check)
        {
            //拦截None
            if (ChecksumType == ChecksumTypes.None) return true;

            //数据或校验为空 抛出
            if (data.Length <= 0 || check.Length <= 0) throw new ArgumentNullException(nameof(data) + nameof(check));

            //拦截Fixed
            string checksumTypeStr = ChecksumType.ToString();
            if (checksumTypeStr.Contains("Fixed")) return GetFixedChecksumValue(checksumTypeStr) == check[0];

            //其他校验方式
            MethodInfo methodInfo = typeof(Checksumer).GetMethod(checksumTypeStr);
            if (methodInfo == null) throw new NotImplementedException(nameof(checksumTypeStr));
            object resultObj = methodInfo.Invoke(this, new object[] { data, startIndex, endIndex, check });
            //转换最终结果
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, resultObj);
                byte[] result = ms.GetBuffer();
                //对比校验值
                return result.SequenceEqual(check);
            }
        }

        /// <summary>
        /// CRC-8 计算
        /// </summary>
        /// <param name="check_data"></param>
        /// <param name="num_of_data"></param>
        /// <returns></returns>
        private static byte[] CRC_8(byte[] check_data, int num_of_data)
        {
            byte bit_mask;        // bit mask
            byte crc = 0xFF; // calculated checksum
            byte byteCtr;    // byte counter

            // calculates 8-Bit checksum with given polynomial
            for (byteCtr = 0; byteCtr < num_of_data; byteCtr++)
            {
                crc ^= (check_data[byteCtr]);
                //crc校验，最高位是1就^0x31
                for (bit_mask = 8; bit_mask > 0; --bit_mask)
                {
                    if ((crc & 0x80) != 0)
                    {
                        crc = (byte)((crc << 1) ^ 0x31);
                    }
                    else
                    {
                        crc = ((byte)(crc << 1));
                    }
                }
            }
            return new byte[] { crc };
        }
    }
}

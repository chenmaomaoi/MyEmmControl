using System;
using System.Linq;
using System.Windows;
using MyEmmControl.Communication.Checksum;
using MyEmmControl.Extensions;

namespace MyEmmControl.Communication
{
    public abstract class CommunicationBase : ICommunication
    {
        protected Checksumer _checksumer;

        public ChecksumTypes ChecksumType
        {
            get => _checksumer.ChecksumType;
            set => _checksumer.ChecksumType = value;
        }

        protected CommunicationBase(ChecksumTypes checksumTypes)
        {
            _checksumer = new Checksumer(checksumTypes);
            ChecksumType = checksumTypes;
        }

        /// <summary>
        /// 校验并且去除校验数据
        /// </summary>
        /// <param name="sdata"> </param>
        /// <param name="dataBody"> </param>
        /// <remarks> 收到数据应先调用改方法校验并去除附加的校验数据 </remarks>
        /// <returns> </returns>
        protected byte[] CheckData(byte[] sdata)
        {
            //校验数据
            byte[] dataBody;
            if (_checksumer.ChecksumType != ChecksumTypes.None)
            {
                int checkDataLength = _checksumer.ChecksumType.GetFieldAttribute<ChecksumDataLengthAttribute>().Length;
                //截取数据，计算校验值
                dataBody = sdata.Take(sdata.Length - checkDataLength).ToArray();
                byte[] checksumData = _checksumer.Calculate(dataBody);
                //截取校验值
                byte[] checkData = sdata.Reverse().Take(checkDataLength).Reverse().ToArray();
                //对比校验值
                if (!checksumData.SequenceEqual(checkData))
                {
                    //校验不通过
                    //todo:写入日志
                    return null;
                }
            }
            else
            {
                dataBody = sdata;
            }
            return dataBody;
        }

        /// <inheritdoc/>
        /// <remarks> 在调用之前应调用CheckData </remarks>
        public abstract event EventHandler<byte[]> OnRecvdData;

        public abstract bool ConnectDeviceAndSettingWindow(Window owner);

        public abstract void Dispose();

        /// <remarks> 拿到返回的数据之后，应在使用数据钱调用CheckData以去除校验信息 </remarks>
        /// <inheritdoc/>
        public abstract byte[] Get(byte[] data);

        /// <remarks> 在发送之前应在数据结尾添加校验 </remarks>
        /// <inheritdoc/>
        public abstract void Send(byte[] data);
    }
}
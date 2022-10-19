using System;

namespace MyEmmControl.Communication
{
    internal interface ICommunication
    {
        void Send(byte[] data);

        void Dispose();

        event EventHandler<byte[]> OnRecvdData;
    }
}

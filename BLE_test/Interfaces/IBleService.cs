using System;
using System.Collections.Generic;
using System.Text;

namespace BLE_test.Interfaces
{
    public interface IBleService
    {
        void ConnectToDevice(IBleDevice device);
        event EventHandler<BleDataEventArgs> DataReceived;
    }

    public class BleDataEventArgs : EventArgs
    {
        public byte[] Data { get; }

        public BleDataEventArgs(byte[] data)
        {
            Data = data;
        }
    }
}

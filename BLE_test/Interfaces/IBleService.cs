using System;
using System.Collections.Generic;
using System.Text;

namespace BLE_test.Interfaces
{
    public interface IBleService
    {
        void ConnectToDevice(IBleDevice device);
        event EventHandler<BleDataEventArgs> DataReceived;
        public event EventHandler<string> ScanTimeoutReached;
        public event EventHandler<string> ConnectionLost;
        public event EventHandler<string> ConnectionError;
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

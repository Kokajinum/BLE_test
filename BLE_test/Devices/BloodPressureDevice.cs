using BLE_test.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLE_test.Devices
{
    public class BloodPressureDevice : IBleDevice
    {
        public string DeviceName => "BM77";
        public Guid ServiceUuid => new Guid("00001810-0000-1000-8000-00805f9b34fb");
        public Guid CharacteristicUuid => new Guid("00002a35-0000-1000-8000-00805f9b34fb");
        public Guid? DescriptorUuid => new Guid("00002902-0000-1000-8000-00805f9b34fb");

        public bool EnableNotificationValue => false;

        public bool EnableIndicationValue => true;

        public void HandleData(byte[] data)
        {
            // Implement data handling logic specific to blood pressure measurements
            // E.g., parse the data and convert to meaningful values
        }
    }
}

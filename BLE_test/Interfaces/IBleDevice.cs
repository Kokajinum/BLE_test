using System;
using System.Collections.Generic;
using System.Text;

namespace BLE_test.Interfaces
{
    public interface IBleDevice
    {
        string DeviceName { get; }
        Guid ServiceUuid { get; }
        Guid CharacteristicUuid { get; }
        Guid? DescriptorUuid { get; }
        bool EnableNotificationValue { get; }
        bool EnableIndicationValue { get; }
        void HandleData(byte[] data);
    }
}

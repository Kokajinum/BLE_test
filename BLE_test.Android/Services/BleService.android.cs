using Android.App;
using Android.Bluetooth.LE;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BLE_test.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Bluetooth.BluetoothAdapter;
using Java.Util;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(BLE_test.Droid.Services.BleService))]
namespace BLE_test.Droid.Services
{
    public class BleService : IBleService
    {
        private BluetoothManager _bluetoothManager;
        private BluetoothAdapter _bluetoothAdapter;
        private BluetoothLeScanner _scanner;
        private ScanCallback _scanCallback;
        private BluetoothGatt _bluetoothGatt;
        private IBleDevice _bleDevice;
        private Handler _scanHandler;
        private const long ScanTimeout = 10000;

        public event EventHandler<BleDataEventArgs> DataReceived;
        public event EventHandler<string> ScanTimeoutReached;
        public event EventHandler<string> ConnectionLost;
        public event EventHandler<string> ConnectionError;

        public BleService()
        {
            _bluetoothManager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService);
            _bluetoothAdapter = _bluetoothManager.Adapter;
            _scanner = _bluetoothAdapter.BluetoothLeScanner;
            _scanHandler = new Handler(Looper.MainLooper);
        }

        public void ConnectToDevice(IBleDevice device)
        {
            _bleDevice = device;
            var scanFilter = new ScanFilter.Builder()
                .SetDeviceName(device.DeviceName)
                .Build();

            var scanSettings = new ScanSettings.Builder()
                .SetScanMode(Android.Bluetooth.LE.ScanMode.LowLatency)
                .Build();

            _scanCallback = new BleScanCallback(this);
            _scanner.StartScan(new[] { scanFilter }, scanSettings, _scanCallback);

            // Set a timeout for scanning
            _scanHandler.PostDelayed(() =>
            {
                _scanner.StopScan(_scanCallback);
                ScanTimeoutReached?.Invoke(this, "Scan timeout reached, no device found.");
            }, ScanTimeout);
        }

        private class BleScanCallback : ScanCallback
        {
            private readonly BleService _bleService;

            public BleScanCallback(BleService bleService)
            {
                _bleService = bleService;
            }

            public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
            {
                base.OnScanResult(callbackType, result);
                if (result.Device.Name != null && result.Device.Name.Contains("BM77"))
                {
                    _bleService._scanner.StopScan(this);
                    _bleService._scanHandler.RemoveCallbacksAndMessages(null); // Stop timeout handler
                    _bleService._bluetoothGatt = result.Device.ConnectGatt(Android.App.Application.Context, false, new GattCallback(_bleService));
                }
            }

            public override void OnScanFailed(ScanFailure errorCode)
            {
                base.OnScanFailed(errorCode);
                _bleService.ConnectionError?.Invoke(_bleService, $"Scan failed: {errorCode}");
            }
        }

        private class GattCallback : BluetoothGattCallback
        {
            private readonly BleService _bleService;

            public GattCallback(BleService bleService)
            {
                _bleService = bleService;
            }

            public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
            {
                if (newState == ProfileState.Connected)
                {
                    gatt.DiscoverServices();
                }
                else if (newState == ProfileState.Disconnected)
                {
                    _bleService.ConnectionLost?.Invoke(_bleService, "Connection lost.");
                }

                // Handle possible connection errors
                if (status != GattStatus.Failure)
                {
                    _bleService.ConnectionError?.Invoke(_bleService, $"Connection error: {status}");
                }
            }

            public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
            {
                var service = gatt.Services.FirstOrDefault(s => s.Uuid == UUID.FromString("00001810-0000-1000-8000-00805f9b34fb"));
                if (service != null)
                {
                    var characteristic = service.GetCharacteristic(UUID.FromString(_bleService._bleDevice.CharacteristicUuid.ToString()));
                    if (characteristic != null)
                    {
                        _bleService._bluetoothGatt.SetCharacteristicNotification(characteristic, true);
                        var descriptorUuid = _bleService._bleDevice.DescriptorUuid;
                        if (descriptorUuid.HasValue)
                        {
                            var descriptor = characteristic.GetDescriptor(UUID.FromString(descriptorUuid.ToString()));
                            //https://developer.android.com/reference/android/bluetooth/BluetoothGattDescriptor#setValue(byte[])
                            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Tiramisu)
                            {
                                descriptor.SetValue(BluetoothGattDescriptor.EnableIndicationValue.ToArray());
                                _bleService._bluetoothGatt.WriteDescriptor(descriptor);
                            }
                            else
                            {
                                _bleService._bluetoothGatt.WriteDescriptor(descriptor, BluetoothGattDescriptor.EnableIndicationValue.ToArray());
                            }
                        }
                    }
                }
            }

            public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, byte[] bytes)
            {
                if (characteristic.Uuid == UUID.FromString(_bleService._bleDevice.CharacteristicUuid.ToString()))
                {
                    _bleService._bleDevice.HandleData(bytes);
                    _bleService.DataReceived?.Invoke(_bleService, new BleDataEventArgs(bytes));
                }
            }
        }
    }
}
using BLE_test.Devices;
using BLE_test.Interfaces;
using BLE_test.Services;
using BLE_test.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BLE_test
{
    public partial class App : Application
    {
        private IBleService bleService;

        public App()
        {
            InitializeComponent();

            bleService = DependencyService.Get<IBleService>();
            var bloodPressureDevice = new BloodPressureDevice();
            bleService.DataReceived += OnDataReceived;
            bleService.ConnectToDevice(bloodPressureDevice);

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        private void OnDataReceived(object sender, BleDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

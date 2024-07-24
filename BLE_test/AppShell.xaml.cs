using BLE_test.ViewModels;
using BLE_test.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BLE_test
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}

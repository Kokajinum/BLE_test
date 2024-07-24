using BLE_test.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace BLE_test.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
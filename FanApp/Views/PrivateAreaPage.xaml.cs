using FanApp.Models;
using FanApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FanApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrivateAreaPage : ContentPage
    {
        public UserViewModel viewModel = null;
        public PrivateAreaPage(UserViewModel userView)
        {
            viewModel = userView;
            BindingContext = viewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            //if (Navigation.NavigationStack.Count > 1) 
            //Navigation.RemovePage(Navigation.NavigationStack.First());
        }

        public void UpdateUser(UserModel user)
        {
            viewModel.user = user;
            BindingContext = viewModel;
        }

        void OnPhoneNumberTapped(object sender, EventArgs args)
        {
            PhoneDialer.Open("+34917297922");
        }

        private async void Discounts_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Descuentos", "Esta opción está en construcción", "Aceptar");
        }

        private async void Notifications_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Notificaciones", "Esta opción está en construcción", "Aceptar");
        }

        private async void Links_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Links de interés", "Esta opción está en construcción", "Aceptar");
        }
        private async void Profile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new AboutPage(new AboutViewModel(viewModel.user) { Opener = this })));
        }
    }
}
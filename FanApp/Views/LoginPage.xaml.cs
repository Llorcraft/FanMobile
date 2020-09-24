using Acr.UserDialogs;
using FanApp.Services;
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
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
#if DEBUG
            email.Text = "luis.llorca@gmail.com";
            password.Text = "lolo";
#endif
        }

        async void OnRegisterTapped(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new NavigationPage(new AboutPage(new AboutViewModel())));
        }

        void OnPhoneNumberTapped(object sender, EventArgs args)
        {
            PhoneDialer.Open("+34917297922");
        }

        async void OnButtonLoginClicked(object sender, EventArgs args)
        {
            UserDialogs.Instance.ShowLoading("Iniciando sesión...");
            var user = await new DataStore().Login(new Models.UserModel { Email = email.Text, Password = password.Text });
            UserDialogs.Instance.HideLoading();
            if (null == user)
            {
                await DisplayAlert("Login", "Correo electrónico y/o contraseña incorrectos", "Aceptar");
            }
            else
            {
                var form = new PrivateAreaPage(new UserViewModel(user));
                await Navigation.PushModalAsync(new NavigationPage(form), false);
            }
        }
    }
}
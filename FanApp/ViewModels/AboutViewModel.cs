using FanApp.Models;
using FanApp.Views;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FanApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public UserModel user;
        public PrivateAreaPage Opener;

        public AboutViewModel()
        {
            Title = "Registro";
        }

        public AboutViewModel(UserModel userModel)
        {
            user = userModel;
            Title = "Mi perfil";
        }

        public string PrivacyPolicy { get; private set; }
        public ICommand OpenWebCommand { get; } = 
            new Command(async () => await Browser.OpenAsync("https://ciudadraqueta.com/politica-privacidad/"));
    }
}
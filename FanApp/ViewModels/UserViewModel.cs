
using FanApp.Models;

namespace FanApp.ViewModels
{
    public class UserViewModel : BaseViewModel

    {
        public UserModel user { get; set; }
        
        public UserViewModel() { }
        public UserViewModel(UserModel userModel)
        {
            user = userModel;

        }
        public string Welcome
        {
            get
            {
                return "Bienvenido(a)";
                //return user.Title.StartsWith("Sr", System.StringComparison.InvariantCultureIgnoreCase) ? "Bienvenido" : "Bienvenida";
            }
        }
    }
}

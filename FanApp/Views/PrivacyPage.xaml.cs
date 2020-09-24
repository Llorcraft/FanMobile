using Acr.UserDialogs;
using HtmlAgilityPack;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FanApp.Views
{
    [DesignTimeVisible(false)]
    public partial class PrivacyPage : ContentPage
    {
        public bool Loading { get; set; } = true;
        public PrivacyPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        public Task<string> LoadHtml()
        {
            return Task.Run(() =>
            {
                //indicator.IsVisible = indicator.IsRunning = true;
                UserDialogs.Instance.ShowLoading("Cargando políticas...");
                var web = new HtmlWeb();
                var doc = web.Load("https://ciudadraqueta.com/politica-privacidad/");
                //doc.DocumentNode.SelectNodes("//li").ToList().ForEach(li => {
                //    li.SetAttributeValue("style", "margin-bottom: 20px");
                //});
                var content = doc.DocumentNode.SelectSingleNode("//div[@class='col-inner']");
                content.Descendants("a").ToList().ForEach(a => a.Attributes.Remove("href"));
                content.Descendants("p").First().Remove();

                //indicator.IsVisible = indicator.IsRunning = false;
                UserDialogs.Instance.HideLoading();
                return content.InnerHtml;
            });
        }

        public void SetHtml(string text)
        {
            html.Text = text;
        }

        protected override void OnAppearing()
        {
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}
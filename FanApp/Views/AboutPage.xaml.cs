using System;
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Essentials;
using FanApp.ViewModels;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using FanApp.Models;
using FanApp.Services;
using Acr.UserDialogs;

namespace FanApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public ICommand ShowPrivacyPolicy { get; private set; }
        public AboutViewModel viewModel = null;
        public AboutPage(AboutViewModel view)
        {
            InitializeComponent();
            viewModel = view;
            BindingContext = viewModel;

            new[] { "Ninguno", "Sr.", "Sra.", "Srta." }
                .ToList()
                .ForEach(i => title.Items.Add(i));

            title.SelectedIndex = 0;
            if (viewModel.user == null) return ;

            gridConditions.IsVisible = false;
            send.Text = "Modificar";
            title.SelectedIndex = title.Items.IndexOf(viewModel.user.Title);
            birthday.Date = viewModel.user.Birthday;
            firstName.Text = viewModel.user.FirstName;
            lastName.Text = viewModel.user.LastName;
            email.Text = viewModel.user.Email;
            phone.Text = viewModel.user.Phone;
            postalCode.Text = viewModel.user.PostalCode;
            comments.Text = viewModel.user.Comments;
            password.Text = confirmPassword.Text = viewModel.user.Password;
        }

        async void OnShowPrivacyPolicyTapped(object sender, EventArgs args)
        {
            var form = new PrivacyPage();
            await Navigation.PushModalAsync(new NavigationPage(form), true);
            form.SetHtml(await form.LoadHtml());
        }

        void OnPhoneNumberTapped(object sender, EventArgs args)
        {
            PhoneDialer.Open("+34917297922");
        }

        void OnCancel(object sender, EventArgs args)
        {
            Navigation.PopModalAsync(true);
        }

        void OnTextChanged(object sender, EventArgs args)
        {
            Entry elm = sender as Entry;
            if (elm.BackgroundColor != Color.FromRgba(255, 0, 0, .1)) return;
            if (!string.IsNullOrEmpty(elm.Text)) elm.BackgroundColor = Color.Transparent;
        }

        void OnEmailChanged(object sender, EventArgs args)
        {
            Entry elm = sender as Entry;
            if (elm.BackgroundColor != Color.FromRgba(255, 0, 0, .1)) return;
            if (!string.IsNullOrEmpty(elm.Text) && Regex.IsMatch(elm.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                elm.BackgroundColor = Color.Transparent;
        }

        void OnConfirmPasswordChanged(object sender, EventArgs args)
        {
            Entry elm = sender as Entry;
            if (elm.BackgroundColor != Color.FromRgba(255, 0, 0, .1)) return;
            if (elm.Text == password.Text) elm.BackgroundColor = Color.Transparent;
        }


        async void OnButtonSendClicked(object sender, EventArgs args)
        {
            View invalid = null;

            Func<View, View> setInvalid = (View elm) =>
            {
                elm.BackgroundColor = Color.FromRgba(255, 0, 0, .1);
                invalid = invalid == null ? elm : invalid;
                return elm;
            };

            if (string.IsNullOrEmpty(firstName.Text)) setInvalid(firstName);
            if (string.IsNullOrEmpty(lastName.Text)) setInvalid(lastName);
            if (string.IsNullOrEmpty(email.Text) || !Regex.IsMatch(email.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase)) setInvalid(email);
            if (string.IsNullOrEmpty(phone.Text)) setInvalid(phone);
            if (string.IsNullOrEmpty(birthday.Date.ToString())) setInvalid(birthday);
            if (string.IsNullOrEmpty(password.Text)) setInvalid(password);
            if (password.Text != confirmPassword.Text) setInvalid(confirmPassword);

            var msg = new List<string>();
            if (!accept.IsChecked && viewModel.user == null) msg.Add("Debe aceptar los terminos y condiciones.");

            if (null != invalid || (!accept.IsChecked && viewModel.user == null))
            {
                if (null != invalid)
                {
                    msg.Add("Revise los campos resaltados en rojo.");
                    invalid.Focus();
                }
                await DisplayAlert("Registro", string.Join("\n", msg), "Aceptar");
            }
            else
            {
                try
                {
                    UserDialogs.Instance.ShowLoading("Registrando...");
                    var user = new UserModel
                    {
                        Title = title.SelectedItem.ToString(),
                        Birthday = birthday.Date,
                        FirstName = firstName.Text,
                        LastName = lastName.Text,
                        Email = email.Text,
                        Phone = phone.Text,
                        PostalCode = postalCode.Text,
                        Comments = comments.Text,
                        Password = password.Text,
                        Id = viewModel.user != null ? viewModel.user.Id : null
                    };
                    var result = await new DataStore().Register(user, viewModel.user == null);

                    if (viewModel.user == null)
                    {
                        SendEmail(viewModel.user ?? result);
                    }
                    else
                    {
                        await Navigation.PopToRootAsync(true);
                    }
                    var form = new PrivateAreaPage(new UserViewModel(result));
                    await Navigation.PushModalAsync(new NavigationPage(form), true);

                } catch (Exception ex)
                {
                    await DisplayAlert(viewModel.user != null ? "Modificar" : "Registro", ex.Message, "Aceptar");
                }
                finally
                {
                    UserDialogs.Instance.HideLoading();
                }
            }
        }

        private void SendEmail(UserModel user)
        {
            string to = "contacts@themobiletoaster.com";
            string from = "no-reply@ciudadraqueta.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Formulario Zona VIP";
            message.Body = string.Join("\n", new string[] {
                $"tratamiento:{title.SelectedItem}",
                $"name:{firstName.Text}",
                $"apellido:{lastName.Text}",
                $"email:{email.Text}",
                $"telefono:{phone.Text}",
                $"fecha:{birthday.Date.ToShortDateString()}",
                $"codigo-postal:{postalCode.Text}",
                $"observaciones:{comments.Text}",
                $"politica-privacidad:Aceptado: He leído y acepto la Política de Privacidad.",
                $"fuente:https://ciudadraqueta.com/zona_vip/" }
            );

            SmtpClient client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("contacts@themobiletoaster.com", "fandatamanager2020!!")
            };
            // Credentials are necessary if the server requires the client
            // to authenticate before it will send email on the client's behalf.
            //client.UseDefaultCredentials = true;

            try
            {
                client.Send(message);
                }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}", ex.ToString());
            }
        }
    }
}
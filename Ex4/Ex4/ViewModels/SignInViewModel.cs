using Ex4.Modele;
using Ex4.Views;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ex4.ViewModels
{
	public class SignInViewModel : ViewModelBase{
        public string TitleLabel { get; set; }

        public ICommand SigninCommand { get; set; }

        private string _email;
        public string Email{
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password{
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _passwordTwo;
        public string PasswordTwo{
            get => _passwordTwo;
            set => SetProperty(ref _passwordTwo, value);
        }

        private string _firstName;
        public string FirstName{
            get => _firstName;
            set => SetProperty(ref this._firstName, value);
        }

        private string _lastName;
        public string LastName{
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public SignInViewModel(){
            TitleLabel = "Création d'un nouveau compte";
            SigninCommand = new Command(SigninClicked);
        }

        private void SigninClicked(object _){
            Signin();
        }

        private async void Signin(){
            if (!string.IsNullOrWhiteSpace(Email)){
                if (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName)){
                    if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(PasswordTwo)){
                        if ((Password == PasswordTwo)){
                            (Boolean test, string message) = await RestService.Rest.SignIn(Email, Password, FirstName, LastName);
                            if (test){
                                await Application.Current.MainPage.DisplayAlert("Inscription", message, "OK");

                                await NavigationService.PopAsync();
                                await NavigationService.PushAsync<MainPage>(new Dictionary<string, object>());
                            }
                            else{
                                await Application.Current.MainPage.DisplayAlert("Inscription", message, "OK");
                            }
                        }
                        else{ // Password != PasswordTwo
                            await Application.Current.MainPage.DisplayAlert("Inscription", "La vérification du mot de passe à échouée.", "OK");
                        }
                    }
                    else{ // Problème password or passwordTwo
                        await Application.Current.MainPage.DisplayAlert("Inscription", "Votre mot de passe ne peut être vide.", "OK");
                    }
                }
                else{ // Problème firstName ou lastName
                    await Application.Current.MainPage.DisplayAlert("Inscription", "Votre nom et prénom ne peuvent être vide.", "OK");
                }
            }
            else{ // Problème email
                await Application.Current.MainPage.DisplayAlert("Inscription", "Votre adresse email ne peut être vide.", "OK");
            }
            Password = "";
            PasswordTwo = "";
        }
    }
}
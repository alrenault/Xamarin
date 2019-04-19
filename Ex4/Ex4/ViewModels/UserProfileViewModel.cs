using Ex4.Modele;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ex4.ViewModels
{
	public class UserProfileViewModel : ViewModelBase{

        private string _titleLabel;
        public string TitleLabel
        {
            get => _titleLabel;
            set => SetProperty(ref _titleLabel, value);
        }

        private User _user;
        public User User{
            get{
                return _user;
            }
            set{
                SetProperty(ref _user, value);
                TitleLabel = "Profil : " + _user.FirstName + " " + _user.LastName;
            }
        }

        public Command NewPhoto { get; private set; }
        public Command NewImage { get; private set; }
        public Command PatchPassword { get; private set; }

        private string _oldPassword;
        public string OldPassword{
            get{
                return _oldPassword;
            }
            set{
                SetProperty(ref _oldPassword, value);
            }
        }

        private string _newPassword;
        public string NewPassword{
            get
            {
                return _newPassword;
            }
            set{
                SetProperty(ref _newPassword, value);
            }
        }

        private string _newPasswordBis;
        public string NewPasswordBis{
            get{
                return _newPasswordBis;
            }
            set{
                SetProperty(ref _newPasswordBis, value);
            }
        }

        public UserProfileViewModel()
        {
            PatchPassword = new Command(PacthPasswordClicked);
        }

        private void PacthPasswordClicked(object _){
            PatchPwd();
        }

        private async void PatchPwd(){
            if (!string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(NewPasswordBis) && !string.IsNullOrWhiteSpace(OldPassword)){
                if (NewPassword == NewPasswordBis){
                    (Boolean test, string message) = await RestService.Rest.PatchPassword(OldPassword, NewPassword);

                    if (test){
                        await Application.Current.MainPage.DisplayAlert("Modification du mot de passe", message, "OK");
                    }
                    else{
                        await Application.Current.MainPage.DisplayAlert("Modification du mot de passe", message, "OK");
                    }
                }
                else{ // if NewPasswordBis is incorrect
                    await Application.Current.MainPage.DisplayAlert("Modification du mot de passe", "Vous devez copier identiquement le nouveau mot de passe deux fois.", "OK");
                }
            }
            else{ // Problem : password size
                await Application.Current.MainPage.DisplayAlert("Modification du mot de passe", "Votre mot de passe ne peut être vide.", "OK");
            }

            OldPassword = "";
            NewPassword = "";
            NewPasswordBis = "";
        }



        public override async Task OnResume(){
            await base.OnResume();
            (Boolean test, User data) = await RestService.Rest.GetUserData();

            if (test){
                User = data;
            }
            else{
                // TODO Deco ?
            }
        }

    }
}
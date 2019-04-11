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
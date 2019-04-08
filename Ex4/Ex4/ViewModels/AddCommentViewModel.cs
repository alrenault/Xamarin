using Ex4.Modele;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ex4.ViewModels
{
	public class AddCommentViewModel : ViewModelBase{

        public string TitleLabel { get; set; }

        private string _myComment;
        public string MyComment{
            get => _myComment;
            set => SetProperty(ref _myComment, value);
        }

        private long _placeId;
        [NavigationParameter]
        public long PlaceId{
            get { return _placeId; }
            set{
                SetProperty(ref _placeId, value);
            }
        }

        public ICommand AddCommand { get; set; }

        public AddCommentViewModel(){
            AddCommand = new Command(AddClicked);
            TitleLabel = "Nouveau commentaire";
        }

        private void AddClicked(){
            AddComment();
        }

        private async void AddComment(){
            if (!string.IsNullOrWhiteSpace(MyComment)){
                (Boolean test, string texte) = await RestService.Rest.AddComment(PlaceId, MyComment);
                if (test){
                    await Application.Current.MainPage.DisplayAlert("Commentaire ajouté", texte, "OK");
                }
                else{
                    await Application.Current.MainPage.DisplayAlert("Erreur", texte, "OK");
                }
                await NavigationService.PopAsync();
            }
            else{
                await Application.Current.MainPage.DisplayAlert("Commentaire éronné", "Votre commentaire ne peut être vide.", "OK");
            }
        }

        public override async Task OnResume(){
            await base.OnResume();
        }
    }
}
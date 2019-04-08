using Ex4.Modele;
using Ex4.Views;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Ex4.ViewModels
{
	public class DetailPlaceViewModel : ViewModelBase{

        private string _titleLabel;
        public string TitleLabel{
            get => _titleLabel;
            set => SetProperty(ref _titleLabel, value);
        }

        private long _placeId;
        [NavigationParameter]
        public long PlaceId{
            get { return _placeId; }
            set{
                SetProperty(ref _placeId, value);
            }
        }
       

        private PlaceModel _place;
        public PlaceModel Place{
            get { return _place; }
            set{
                SetProperty(ref _place, value);
                Pins = new ObservableCollection<Pin>() {
                    new Pin(){
                            Position = Place.Position,
                            Label = Place.Title
                        }
                };
            }
        }

        private IList<Pin> _pins;
        public IList<Pin> Pins{
            get{
                return _pins;
            }
            set{
                SetProperty(ref _pins, value);
            }
        }

        public ICommand RefreshCommand { get; set; }

        private bool _isRefreshing = false;
        public bool IsRefreshing{
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand AddCommentCommand { get; set; }


        public DetailPlaceViewModel(){
            RefreshCommand = new Command(RefreshClicked);
            AddCommentCommand = new Command(AddCommentClicked);
        }

        private void RefreshClicked(){
            RefreshPlace();
            IsRefreshing = false;
        }

        private async void RefreshPlace(){
            Place = await RestService.Rest.LoadPlace(PlaceId);
        }

        private void AddCommentClicked(){
            OpenAddComment();
        }

        private async void OpenAddComment(){
            Debug.WriteLine("Je passe par la .........................");
            await NavigationService.PushAsync<AddComment>(new Dictionary<string, object>() { { "PlaceId", Place.Id } });
        }

        public override async Task OnResume(){
            await base.OnResume();
            Place = await RestService.Rest.LoadPlace(PlaceId);
        }

    }
}